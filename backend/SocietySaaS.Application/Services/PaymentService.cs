using SocietySaaS.Application.Common.DTOs;
using SocietySaaS.Application.Common.Interfaces;
using SocietySaaS.Application.Common.Models;
using SocietySaaS.Domain.Entities;
using SocietySaaS.Domain.Interfaces;

namespace SocietySaaS.Application.Services;

public interface IPaymentService
{
    Task<PaginatedList<PaymentDto>> GetAllAsync(int page, int pageSize, Guid? flatId);
    Task<object?> GetByIdAsync(Guid id);
    Task<PaymentDto> CreateAsync(CreatePaymentRequest request);
    Task<PaymentDto> ReverseAsync(Guid id, ReversePaymentRequest request);
}

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IBillRepository _billRepository;
    private readonly IFlatRepository _flatRepository;
    private readonly IReceiptRepository _receiptRepository;
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public PaymentService(
        IPaymentRepository paymentRepository,
        IBillRepository billRepository,
        IFlatRepository flatRepository,
        IReceiptRepository receiptRepository,
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _paymentRepository = paymentRepository;
        _billRepository = billRepository;
        _flatRepository = flatRepository;
        _receiptRepository = receiptRepository;
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<PaginatedList<PaymentDto>> GetAllAsync(int page, int pageSize, Guid? flatId)
    {
        var payments = await _paymentRepository.GetAllWithDetailsAsync();
        var tenantId = _currentUser.TenantId;

        var filtered = payments.Where(p => p.TenantId == tenantId && !p.IsDeleted);
        if (flatId.HasValue)
            filtered = filtered.Where(p => p.FlatId == flatId.Value);

        var total = filtered.Count();
        var paged = filtered
            .OrderByDescending(p => p.PaymentDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new PaymentDto(p.Id, p.PaymentNumber, p.PaymentDate, p.Amount, p.PaymentMode, p.TransactionReference, p.Notes, p.Status, p.FlatId, p.Flat?.FlatNumber))
            .ToList();

        return new PaginatedList<PaymentDto>(paged, total, page, pageSize);
    }

    public async Task<object?> GetByIdAsync(Guid id)
    {
        var payment = await _paymentRepository.GetWithDetailsAsync(id);
        if (payment == null) return null;

        return new
        {
            payment.Id, payment.PaymentNumber, payment.PaymentDate, payment.Amount,
            payment.PaymentMode, payment.TransactionReference, payment.Notes,
            payment.Status, payment.FlatId, FlatNumber = payment.Flat?.FlatNumber,
            Allocations = payment.PaymentAllocations?.Select(pa => new PaymentAllocationDto(pa.Id, pa.Amount, pa.PaymentId, pa.BillId, pa.Bill?.BillNumber))
        };
    }

    public async Task<PaymentDto> CreateAsync(CreatePaymentRequest request)
    {
        var tenantId = _currentUser.TenantId!.Value;
        var flat = await _flatRepository.GetByIdAsync(request.FlatId);
        if (flat == null || flat.TenantId != tenantId) throw new KeyNotFoundException("Flat not found");

        var paymentNumber = $"PAY-{DateTime.UtcNow:yyyyMMdd}-{flat.FlatNumber}-{DateTime.UtcNow:HHmmss}";

        var paymentId = Guid.NewGuid();
        var payment = new Payment
        {
            Id = paymentId,
            TenantId = tenantId,
            PaymentNumber = paymentNumber,
            PaymentDate = request.PaymentDate,
            Amount = request.Amount,
            PaymentMode = request.PaymentMode,
            TransactionReference = request.TransactionReference,
            Notes = request.Notes,
            FlatId = request.FlatId,
            Status = "Completed"
        };

        var allBills = await _billRepository.GetAllWithDetailsAsync();
        var outstandingBills = allBills
            .Where(b => b.FlatId == request.FlatId && !b.IsDeleted && b.Status != "Cancelled" && b.GrandTotal > b.AmountPaid)
            .OrderBy(b => b.DueDate)
            .ToList();

        decimal remaining = request.Amount;
        foreach (var bill in outstandingBills)
        {
            if (remaining <= 0) break;
            var billOutstanding = bill.GrandTotal - bill.AmountPaid;
            var allocAmount = Math.Min(remaining, billOutstanding);

            payment.PaymentAllocations.Add(new PaymentAllocation
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                PaymentId = paymentId,
                BillId = bill.Id,
                Amount = allocAmount
            });

            bill.AmountPaid += allocAmount;
            bill.BalanceOutstanding = bill.GrandTotal - bill.AmountPaid;
            if (bill.AmountPaid >= bill.GrandTotal)
                bill.Status = "Paid";

            await _billRepository.UpdateAsync(bill);
            remaining -= allocAmount;
        }

        var receipt = new Receipt
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            ReceiptNumber = $"RCT-{DateTime.UtcNow:yyyyMMdd}-{flat.FlatNumber}-{DateTime.UtcNow:HHmmss}",
            ReceiptDate = DateTime.UtcNow,
            Amount = request.Amount,
            PaymentMode = request.PaymentMode,
            TransactionReference = request.TransactionReference,
            PaymentId = paymentId,
            FlatId = request.FlatId
        };
        await _receiptRepository.AddAsync(receipt);

        await _paymentRepository.AddAsync(payment);

        var ledgerEntry = new LedgerEntry
        {
            TransactionDate = DateTime.UtcNow,
            ReferenceType = "Payment",
            ReferenceId = payment.Id,
            Description = $"Payment {paymentNumber}",
            Credit = request.Amount,
            FlatId = request.FlatId,
            TenantId = tenantId
        };
        await _context.Set<LedgerEntry>().AddAsync(ledgerEntry);

        return new PaymentDto(payment.Id, payment.PaymentNumber, payment.PaymentDate, payment.Amount, payment.PaymentMode, payment.TransactionReference, payment.Notes, payment.Status, payment.FlatId, flat.FlatNumber);
    }

    public async Task<PaymentDto> ReverseAsync(Guid id, ReversePaymentRequest request)
    {
        var payment = await _paymentRepository.GetWithDetailsAsync(id);
        if (payment == null) throw new KeyNotFoundException("Payment not found");
        if (payment.IsReversed) throw new InvalidOperationException("Payment already reversed");

        payment.IsReversed = true;
        payment.ReversedAt = DateTime.UtcNow;
        payment.ReversedBy = _currentUser.UserId?.ToString();
        payment.ReversalReason = request.ReversalReason;
        payment.Status = "Reversed";

        foreach (var alloc in payment.PaymentAllocations)
        {
            var bill = await _billRepository.GetByIdAsync(alloc.BillId);
            if (bill != null)
            {
                bill.AmountPaid -= alloc.Amount;
                bill.BalanceOutstanding = bill.GrandTotal - bill.AmountPaid;
                if (bill.Status == "Paid") bill.Status = "Pending";
                await _billRepository.UpdateAsync(bill);
            }
        }

        await _paymentRepository.UpdateAsync(payment);

        return new PaymentDto(payment.Id, payment.PaymentNumber, payment.PaymentDate, payment.Amount, payment.PaymentMode, payment.TransactionReference, payment.Notes, payment.Status, payment.FlatId, payment.Flat?.FlatNumber);
    }
}
