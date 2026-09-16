using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocietySaaS.Application.Common.DTOs;
using SocietySaaS.Application.Common.Interfaces;
using SocietySaaS.Domain.Entities;

namespace SocietySaaS.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public PaymentsController(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] Guid? flatId = null)
    {
        var tenantId = _currentUser.TenantId;
        if (tenantId == null) return BadRequest("Tenant not selected");

        var query = _context.Payments
            .Include(p => p.Flat)
            .Where(p => p.TenantId == tenantId && !p.IsDeleted);

        if (flatId.HasValue)
            query = query.Where(p => p.FlatId == flatId.Value);

        var total = await query.CountAsync();
        var payments = await query
            .OrderByDescending(p => p.PaymentDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new PaymentDto(p.Id, p.PaymentNumber, p.PaymentDate, p.Amount, p.PaymentMode, p.TransactionReference, p.Notes, p.Status, p.FlatId, p.Flat.FlatNumber))
            .ToListAsync();

        return Ok(new { items = payments, total, page, pageSize });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var payment = await _context.Payments
            .Include(p => p.Flat)
            .Include(p => p.PaymentAllocations).ThenInclude(pa => pa.Bill)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (payment == null) return NotFound();

        return Ok(new
        {
            payment.Id, payment.PaymentNumber, payment.PaymentDate, payment.Amount,
            payment.PaymentMode, payment.TransactionReference, payment.Notes,
            payment.Status, payment.FlatId, FlatNumber = payment.Flat?.FlatNumber,
            Allocations = payment.PaymentAllocations.Select(pa => new PaymentAllocationDto(pa.Id, pa.Amount, pa.PaymentId, pa.BillId, pa.Bill?.BillNumber))
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePaymentRequest request)
    {
        var tenantId = _currentUser.TenantId;
        if (tenantId == null) return BadRequest("Tenant not selected");

        var flat = await _context.Flats.FindAsync(request.FlatId);
        if (flat == null || flat.TenantId != tenantId) return NotFound("Flat not found");

        var paymentNumber = $"PAY-{DateTime.UtcNow:yyyyMMdd}-{flat.FlatNumber}-{DateTime.UtcNow:HHmmss}";

        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId.Value,
            PaymentNumber = paymentNumber,
            PaymentDate = request.PaymentDate,
            Amount = request.Amount,
            PaymentMode = request.PaymentMode,
            TransactionReference = request.TransactionReference,
            Notes = request.Notes,
            FlatId = request.FlatId,
            Status = "Completed",
            CreatedAt = DateTime.UtcNow
        };

        _context.Payments.Add(payment);

        // Auto-allocate to oldest outstanding bills
        var outstandingBills = await _context.Bills
            .Where(b => b.FlatId == request.FlatId && !b.IsDeleted && b.Status != "Cancelled" && b.GrandTotal > b.AmountPaid)
            .OrderBy(b => b.DueDate)
            .ToListAsync();

        decimal remaining = request.Amount;
        foreach (var bill in outstandingBills)
        {
            if (remaining <= 0) break;
            var billOutstanding = bill.GrandTotal - bill.AmountPaid;
            var allocAmount = Math.Min(remaining, billOutstanding);

            payment.PaymentAllocations.Add(new PaymentAllocation
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId.Value,
                PaymentId = payment.Id,
                BillId = bill.Id,
                Amount = allocAmount,
                CreatedAt = DateTime.UtcNow
            });

            bill.AmountPaid += allocAmount;
            bill.BalanceOutstanding = bill.GrandTotal - bill.AmountPaid;
            if (bill.AmountPaid >= bill.GrandTotal)
                bill.Status = "Paid";

            remaining -= allocAmount;
        }

        // Create receipt
        var receipt = new Receipt
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId.Value,
            ReceiptNumber = $"RCT-{DateTime.UtcNow:yyyyMMdd}-{flat.FlatNumber}-{DateTime.UtcNow:HHmmss}",
            ReceiptDate = DateTime.UtcNow,
            Amount = request.Amount,
            PaymentMode = request.PaymentMode,
            TransactionReference = request.TransactionReference,
            PaymentId = payment.Id,
            FlatId = request.FlatId,
            CreatedAt = DateTime.UtcNow
        };
        _context.Receipts.Add(receipt);

        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = payment.Id }, payment);
    }

    [HttpPost("{id:guid}/reverse")]
    public async Task<IActionResult> Reverse(Guid id, [FromBody] ReversePaymentRequest request)
    {
        var payment = await _context.Payments
            .Include(p => p.PaymentAllocations)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (payment == null) return NotFound();
        if (payment.IsReversed) return BadRequest("Payment already reversed");

        payment.IsReversed = true;
        payment.ReversedAt = DateTime.UtcNow;
        payment.ReversedBy = _currentUser.UserId?.ToString();
        payment.ReversalReason = request.ReversalReason;
        payment.Status = "Reversed";

        // Reverse allocations
        foreach (var alloc in payment.PaymentAllocations)
        {
            var bill = await _context.Bills.FindAsync(alloc.BillId);
            if (bill != null)
            {
                bill.AmountPaid -= alloc.Amount;
                bill.BalanceOutstanding = bill.GrandTotal - bill.AmountPaid;
                if (bill.Status == "Paid") bill.Status = "Pending";
            }
        }

        await _context.SaveChangesAsync();
        return Ok(payment);
    }
}
