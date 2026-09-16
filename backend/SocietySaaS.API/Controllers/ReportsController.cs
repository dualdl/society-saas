using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocietySaaS.Application.Common.DTOs;
using SocietySaaS.Application.Common.Interfaces;

namespace SocietySaaS.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public ReportsController(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    [HttpGet("revenue")]
    public async Task<IActionResult> GetRevenueReport([FromQuery] string? billingPeriod = null)
    {
        var tenantId = _currentUser.TenantId;
        if (tenantId == null) return BadRequest("Tenant not selected");

        var query = _context.Bills
            .Include(b => b.Flat)
            .Where(b => b.TenantId == tenantId && !b.IsDeleted && b.Status != "Cancelled");

        if (!string.IsNullOrWhiteSpace(billingPeriod))
            query = query.Where(b => b.BillingPeriod == billingPeriod);

        var byFlat = await query
            .GroupBy(b => new { b.Flat.FlatNumber, b.FlatId })
            .Select(g => new RevenueByFlatDto(
                g.Key.FlatNumber,
                g.Sum(b => b.GrandTotal),
                g.Sum(b => b.AmountPaid),
                g.Sum(b => b.GrandTotal - b.AmountPaid)))
            .ToListAsync();

        var totalBilled = byFlat.Sum(x => x.Billed);
        var totalCollected = byFlat.Sum(x => x.Collected);
        var totalOutstanding = byFlat.Sum(x => x.Outstanding);

        return Ok(new RevenueReportDto(totalBilled, totalCollected, totalOutstanding, byFlat));
    }

    [HttpGet("outstanding")]
    public async Task<IActionResult> GetOutstandingReport()
    {
        var tenantId = _currentUser.TenantId;
        if (tenantId == null) return BadRequest("Tenant not selected");

        var bills = await _context.Bills
            .Include(b => b.Flat).ThenInclude(f => f.Members.Where(m => m.IsPrimary))
            .Where(b => b.TenantId == tenantId && !b.IsDeleted && b.Status != "Cancelled" && b.BalanceOutstanding > 0)
            .OrderBy(b => b.DueDate)
            .ToListAsync();

        var flats = bills.Select(b => new OutstandingByFlatDto(
            b.Flat.FlatNumber,
            b.Flat.Members.FirstOrDefault(m => m.IsPrimary) != null
                ? $"{b.Flat.Members.First(m => m.IsPrimary).FirstName} {b.Flat.Members.First(m => m.IsPrimary).LastName}"
                : null,
            b.BalanceOutstanding,
            b.DueDate < DateTime.UtcNow ? (int)(DateTime.UtcNow - b.DueDate).TotalDays : 0))
            .ToList();

        var grandTotal = flats.Sum(f => f.Amount);

        return Ok(new OutstandingReportDto(flats, grandTotal));
    }
}
