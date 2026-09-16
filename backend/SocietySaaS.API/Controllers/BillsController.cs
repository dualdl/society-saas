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
public class BillsController : ControllerBase
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public BillsController(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? status = null, [FromQuery] string? billingPeriod = null, [FromQuery] Guid? flatId = null)
    {
        var tenantId = _currentUser.TenantId;
        if (tenantId == null) return BadRequest("Tenant not selected");

        var query = _context.Bills
            .Include(b => b.Flat)
            .Include(b => b.BillLines).ThenInclude(bl => bl.Charge)
            .Where(b => b.TenantId == tenantId && !b.IsDeleted);

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(b => b.Status == status);
        if (!string.IsNullOrWhiteSpace(billingPeriod))
            query = query.Where(b => b.BillingPeriod == billingPeriod);
        if (flatId.HasValue)
            query = query.Where(b => b.FlatId == flatId.Value);

        var total = await query.CountAsync();
        var bills = await query
            .OrderByDescending(b => b.BillDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(b => new BillDto(
                b.Id, b.BillNumber, b.BillingPeriod, b.BillDate, b.DueDate,
                b.PreviousOutstanding, b.CurrentCharges, b.LateFee, b.Adjustment,
                b.GrandTotal, b.AmountPaid, b.BalanceOutstanding, b.Status,
                b.FlatId, b.Flat.FlatNumber,
                b.BillLines.Select(bl => new BillLineDto(bl.Id, bl.Amount, bl.Description, bl.ChargeId, bl.Charge.Name)).ToList()))
            .ToListAsync();

        return Ok(new { items = bills, total, page, pageSize });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var bill = await _context.Bills
            .Include(b => b.Flat)
            .Include(b => b.BillLines).ThenInclude(bl => bl.Charge)
            .Include(b => b.PaymentAllocations).ThenInclude(pa => pa.Payment)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (bill == null) return NotFound();

        return Ok(new BillDto(
            bill.Id, bill.BillNumber, bill.BillingPeriod, bill.BillDate, bill.DueDate,
            bill.PreviousOutstanding, bill.CurrentCharges, bill.LateFee, bill.Adjustment,
            bill.GrandTotal, bill.AmountPaid, bill.BalanceOutstanding, bill.Status,
            bill.FlatId, bill.Flat?.FlatNumber,
            bill.BillLines.Select(bl => new BillLineDto(bl.Id, bl.Amount, bl.Description, bl.ChargeId, bl.Charge?.Name)).ToList()));
    }

    [HttpPost("generate")]
    public async Task<IActionResult> GenerateBill([FromBody] GenerateBillRequest request)
    {
        var tenantId = _currentUser.TenantId;
        if (tenantId == null) return BadRequest("Tenant not selected");

        var flat = await _context.Flats.FindAsync(request.FlatId);
        if (flat == null || flat.TenantId != tenantId) return NotFound("Flat not found");

        var previousOutstanding = await _context.Bills
            .Where(b => b.FlatId == request.FlatId && !b.IsDeleted && b.Status != "Cancelled")
            .SumAsync(b => b.GrandTotal - b.AmountPaid);

        var currentCharges = request.BillLines.Sum(bl => bl.Amount);
        var grandTotal = previousOutstanding + currentCharges;

        var billNumber = $"BILL-{DateTime.UtcNow:yyyyMMdd}-{flat.FlatNumber}-{DateTime.UtcNow:HHmmss}";

        var bill = new Bill
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId.Value,
            BillNumber = billNumber,
            BillingPeriod = request.BillingPeriod,
            BillDate = DateTime.UtcNow,
            DueDate = request.DueDate,
            PreviousOutstanding = previousOutstanding,
            CurrentCharges = currentCharges,
            GrandTotal = grandTotal,
            BalanceOutstanding = grandTotal,
            FlatId = request.FlatId,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };

        foreach (var line in request.BillLines)
        {
            bill.BillLines.Add(new BillLine
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId.Value,
                BillId = bill.Id,
                ChargeId = line.ChargeId,
                Amount = line.Amount,
                Description = line.Description,
                CreatedAt = DateTime.UtcNow
            });
        }

        _context.Bills.Add(bill);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = bill.Id }, bill);
    }

    [HttpPost("bulk-generate")]
    public async Task<IActionResult> BulkGenerate([FromBody] BulkGenerateBillsRequest request)
    {
        var tenantId = _currentUser.TenantId;
        if (tenantId == null) return BadRequest("Tenant not selected");

        var flats = await _context.Flats
            .Where(f => f.TenantId == tenantId && !f.IsDeleted && f.IsActive)
            .ToListAsync();

        if (request.FlatIds != null && request.FlatIds.Any())
            flats = flats.Where(f => request.FlatIds.Contains(f.Id)).ToList();

        var charges = await _context.Charges
            .Where(c => c.TenantId == tenantId && !c.IsDeleted && c.IsActive && c.IsRecurring)
            .ToListAsync();

        var createdBills = new List<Bill>();

        foreach (var flat in flats)
        {
            var previousOutstanding = await _context.Bills
                .Where(b => b.FlatId == flat.Id && !b.IsDeleted && b.Status != "Cancelled")
                .SumAsync(b => b.GrandTotal - b.AmountPaid);

            var billLines = charges.Select(c => new BillLine
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId.Value,
                ChargeId = c.Id,
                Amount = c.Amount,
                Description = c.Name,
                CreatedAt = DateTime.UtcNow
            }).ToList();

            var currentCharges = billLines.Sum(bl => bl.Amount);
            var grandTotal = previousOutstanding + currentCharges;

            var bill = new Bill
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId.Value,
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
                CreatedAt = DateTime.UtcNow,
                BillLines = billLines
            };

            _context.Bills.Add(bill);
            createdBills.Add(bill);
        }

        await _context.SaveChangesAsync();
        return Ok(new { message = $"{createdBills.Count} bills generated", billCount = createdBills.Count });
    }

    [HttpPut("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] string status)
    {
        var bill = await _context.Bills.FindAsync(id);
        if (bill == null) return NotFound();

        bill.Status = status;
        bill.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return Ok(bill);
    }

    [HttpPut("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id, [FromBody] string reason)
    {
        var bill = await _context.Bills.FindAsync(id);
        if (bill == null) return NotFound();

        bill.Status = "Cancelled";
        bill.IsCancelled = true;
        bill.CancelledAt = DateTime.UtcNow;
        bill.CancelledBy = _currentUser.UserId?.ToString();
        bill.CancellationReason = reason;
        bill.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return Ok(bill);
    }
}
