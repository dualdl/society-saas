using SocietySaaS.Application.Common.DTOs;
using SocietySaaS.Application.Common.Interfaces;
using SocietySaaS.Application.Common.Models;
using SocietySaaS.Domain.Entities;
using SocietySaaS.Domain.Interfaces;

namespace SocietySaaS.Application.Services;

public interface IBillService
{
    Task<PaginatedList<BillDto>> GetAllAsync(int page, int pageSize, string? status, string? billingPeriod, Guid? flatId);
    Task<BillDto?> GetByIdAsync(Guid id);
    Task<BillDto> GenerateAsync(GenerateBillRequest request);
    Task<int> BulkGenerateAsync(BulkGenerateBillsRequest request);
    Task<BillDto> UpdateStatusAsync(Guid id, string status);
    Task<BillDto> CancelAsync(Guid id, string reason);
}

public class BillService : IBillService
{
    private readonly IBillRepository _billRepository;
    private readonly IFlatRepository _flatRepository;
    private readonly IChargeRepository _chargeRepository;
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public BillService(
        IBillRepository billRepository,
        IFlatRepository flatRepository,
        IChargeRepository chargeRepository,
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _billRepository = billRepository;
        _flatRepository = flatRepository;
        _chargeRepository = chargeRepository;
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<PaginatedList<BillDto>> GetAllAsync(int page, int pageSize, string? status, string? billingPeriod, Guid? flatId)
    {
        var bills = await _billRepository.GetAllWithDetailsAsync();
        var tenantId = _currentUser.TenantId;

        var filtered = bills.Where(b => b.TenantId == tenantId && !b.IsDeleted);

        if (!string.IsNullOrWhiteSpace(status))
            filtered = filtered.Where(b => b.Status == status);
        if (!string.IsNullOrWhiteSpace(billingPeriod))
            filtered = filtered.Where(b => b.BillingPeriod == billingPeriod);
        if (flatId.HasValue)
            filtered = filtered.Where(b => b.FlatId == flatId.Value);

        var total = filtered.Count();
        var paged = filtered
            .OrderByDescending(b => b.BillDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(MapToDto)
            .ToList();

        return new PaginatedList<BillDto>(paged, total, page, pageSize);
    }

    public async Task<BillDto?> GetByIdAsync(Guid id)
    {
        var bill = await _billRepository.GetWithDetailsAsync(id);
        if (bill == null) return null;
        return MapToDto(bill);
    }

    public async Task<BillDto> GenerateAsync(GenerateBillRequest request)
    {
        var tenantId = _currentUser.TenantId!.Value;
        var flat = await _flatRepository.GetByIdAsync(request.FlatId);
        if (flat == null || flat.TenantId != tenantId) throw new KeyNotFoundException("Flat not found");

        var allBills = await _billRepository.GetAllWithDetailsAsync();
        var previousOutstanding = allBills
            .Where(b => b.FlatId == request.FlatId && !b.IsDeleted && b.Status != "Cancelled")
            .Sum(b => b.GrandTotal - b.AmountPaid);

        var currentCharges = request.BillLines.Sum(bl => bl.Amount);
        var grandTotal = previousOutstanding + currentCharges;
        var billNumber = $"BILL-{DateTime.UtcNow:yyyyMMdd}-{flat.FlatNumber}-{DateTime.UtcNow:HHmmss}";

        var billId = Guid.NewGuid();
        var bill = new Bill
        {
            Id = billId,
            TenantId = tenantId,
            BillNumber = billNumber,
            BillingPeriod = request.BillingPeriod,
            BillDate = DateTime.UtcNow,
            DueDate = request.DueDate,
            PreviousOutstanding = previousOutstanding,
            CurrentCharges = currentCharges,
            GrandTotal = grandTotal,
            BalanceOutstanding = grandTotal,
            FlatId = request.FlatId,
            Status = "Pending"
        };

        foreach (var line in request.BillLines)
        {
            bill.BillLines.Add(new BillLine
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                BillId = billId,
                ChargeId = line.ChargeId,
                Amount = line.Amount,
                Description = line.Description
            });
        }

        await _billRepository.AddAsync(bill);

        var ledgerEntry = new LedgerEntry
        {
            TransactionDate = DateTime.UtcNow,
            ReferenceType = "Bill",
            ReferenceId = bill.Id,
            Description = $"Bill {billNumber} for {request.BillingPeriod}",
            Debit = grandTotal,
            FlatId = request.FlatId,
            TenantId = tenantId
        };
        await _context.Set<LedgerEntry>().AddAsync(ledgerEntry);

        return MapToDto(bill);
    }

    public async Task<int> BulkGenerateAsync(BulkGenerateBillsRequest request)
    {
        var tenantId = _currentUser.TenantId!.Value;
        var allFlats = await _flatRepository.GetAllWithMembersAsync();
        var flats = allFlats.Where(f => f.TenantId == tenantId && !f.IsDeleted && f.IsActive).ToList();

        if (request.FlatIds != null && request.FlatIds.Any())
            flats = flats.Where(f => request.FlatIds.Contains(f.Id)).ToList();

        var allCharges = await _chargeRepository.GetActiveChargesAsync();
        var charges = allCharges.Where(c => c.TenantId == tenantId && !c.IsDeleted && c.IsActive && c.IsRecurring).ToList();
        var allBills = await _billRepository.GetAllWithDetailsAsync();

        foreach (var flat in flats)
        {
            var previousOutstanding = allBills
                .Where(b => b.FlatId == flat.Id && !b.IsDeleted && b.Status != "Cancelled")
                .Sum(b => b.GrandTotal - b.AmountPaid);

            var billLines = charges.Select(c => new BillLine
            {
                TenantId = tenantId,
                ChargeId = c.Id,
                Amount = c.Amount,
                Description = c.Name
            }).ToList();

            var currentCharges = billLines.Sum(bl => bl.Amount);
            var grandTotal = previousOutstanding + currentCharges;

            var bulkBillId = Guid.NewGuid();
            foreach (var line in billLines)
            {
                line.Id = Guid.NewGuid();
                line.BillId = bulkBillId;
            }

            var bill = new Bill
            {
                Id = bulkBillId,
                TenantId = tenantId,
                BillNumber = $"BILL-{DateTime.UtcNow:yyyyMMdd}-{flat.FlatNumber}-{Guid.NewGuid().ToString()[..6]}",
                BillingPeriod = request.BillingPeriod,
                BillDate = DateTime.UtcNow,
                DueDate = request.DueDate,
                PreviousOutstanding = previousOutstanding,
                CurrentCharges = currentCharges,
                GrandTotal = grandTotal,
                BalanceOutstanding = grandTotal,
                FlatId = flat.Id,
                Status = "Pending",
                BillLines = billLines
            };

            await _billRepository.AddAsync(bill);
        }

        return flats.Count;
    }

    public async Task<BillDto> UpdateStatusAsync(Guid id, string status)
    {
        var bill = await _billRepository.GetByIdAsync(id);
        if (bill == null) throw new KeyNotFoundException("Bill not found");

        bill.Status = status;
        await _billRepository.UpdateAsync(bill);
        return MapToDto(bill);
    }

    public async Task<BillDto> CancelAsync(Guid id, string reason)
    {
        var bill = await _billRepository.GetByIdAsync(id);
        if (bill == null) throw new KeyNotFoundException("Bill not found");

        bill.Status = "Cancelled";
        bill.IsCancelled = true;
        bill.CancelledAt = DateTime.UtcNow;
        bill.CancelledBy = _currentUser.UserId?.ToString();
        bill.CancellationReason = reason;

        await _billRepository.UpdateAsync(bill);
        return MapToDto(bill);
    }

    private static BillDto MapToDto(Bill bill)
    {
        return new BillDto(
            bill.Id, bill.BillNumber, bill.BillingPeriod, bill.BillDate, bill.DueDate,
            bill.PreviousOutstanding, bill.CurrentCharges, bill.LateFee, bill.Adjustment,
            bill.GrandTotal, bill.AmountPaid, bill.BalanceOutstanding, bill.Status,
            bill.FlatId, bill.Flat?.FlatNumber,
            bill.BillLines?.Select(bl => new BillLineDto(bl.Id, bl.Amount, bl.Description, bl.ChargeId, bl.Charge?.Name)).ToList() ?? new());
    }
}
