using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocietySaaS.Application.Common.DTOs;
using SocietySaaS.Application.Common.Interfaces;

namespace SocietySaaS.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public DashboardController(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<IActionResult> GetDashboard()
    {
        var tenantId = _currentUser.TenantId;
        if (tenantId == null) return BadRequest("Tenant not selected");

        var totalFlats = await _context.Flats.CountAsync(f => f.TenantId == tenantId && !f.IsDeleted);
        var occupiedFlats = await _context.Flats.CountAsync(f => f.TenantId == tenantId && !f.IsDeleted && f.OccupancyStatus != "Vacant");
        var totalMembers = await _context.Members.CountAsync(m => m.TenantId == tenantId && !m.IsDeleted && m.IsActive);

        var totalBilled = await _context.Bills
            .Where(b => b.TenantId == tenantId && !b.IsDeleted && b.Status != "Cancelled")
            .SumAsync(b => b.GrandTotal);
        var totalCollected = await _context.Bills
            .Where(b => b.TenantId == tenantId && !b.IsDeleted && b.Status != "Cancelled")
            .SumAsync(b => b.AmountPaid);
        var totalOutstanding = totalBilled - totalCollected;
        var collectionPct = totalBilled > 0 ? Math.Round(totalCollected / totalBilled * 100, 1) : 0;

        var monthlyTrends = await _context.Bills
            .Where(b => b.TenantId == tenantId && !b.IsDeleted && b.Status != "Cancelled")
            .GroupBy(b => b.BillingPeriod)
            .OrderByDescending(g => g.Key)
            .Take(6)
            .Select(g => new MonthlyTrendDto(g.Key, g.Sum(b => b.GrandTotal), g.Sum(b => b.AmountPaid)))
            .ToListAsync();

        var recentPayments = await _context.Payments
            .Include(p => p.Flat)
            .Where(p => p.TenantId == tenantId && !p.IsDeleted)
            .OrderByDescending(p => p.PaymentDate)
            .Take(5)
            .Select(p => new RecentActivityDto(
                $"Payment of ₹{p.Amount:N0} received from Flat {p.Flat.FlatNumber}",
                "Payment", p.PaymentDate))
            .ToListAsync();

        var recentBills = await _context.Bills
            .Include(b => b.Flat)
            .Where(b => b.TenantId == tenantId && !b.IsDeleted)
            .OrderByDescending(b => b.BillDate)
            .Take(5)
            .Select(b => new RecentActivityDto(
                $"Bill {b.BillNumber} generated for Flat {b.Flat.FlatNumber} - ₹{b.GrandTotal:N0}",
                "Bill", b.BillDate))
            .ToListAsync();

        var recentActivities = recentPayments.Concat(recentBills)
            .OrderByDescending(a => a.Timestamp)
            .Take(10)
            .ToList();

        return Ok(new DashboardDto(
            totalFlats, occupiedFlats, totalMembers,
            totalBilled, totalCollected, totalOutstanding,
            collectionPct, monthlyTrends, recentActivities));
    }
}
