using SocietySaaS.Application.Common.DTOs;
using SocietySaaS.Application.Common.Interfaces;
using SocietySaaS.Domain.Interfaces;

namespace SocietySaaS.Application.Services;

public interface IReportService
{
    Task<RevenueReportDto?> GetRevenueReportAsync(string? billingPeriod);
    Task<OutstandingReportDto?> GetOutstandingReportAsync();
}

public class ReportService : IReportService
{
    private readonly IBillRepository _billRepository;
    private readonly ICurrentUserService _currentUser;

    public ReportService(IBillRepository billRepository, ICurrentUserService currentUser)
    {
        _billRepository = billRepository;
        _currentUser = currentUser;
    }

    public async Task<RevenueReportDto?> GetRevenueReportAsync(string? billingPeriod)
    {
        var tenantId = _currentUser.TenantId;
        if (tenantId == null) return null;

        var bills = await _billRepository.GetAllWithDetailsAsync();
        var filtered = bills.Where(b => b.TenantId == tenantId && !b.IsDeleted && b.Status != "Cancelled");

        if (!string.IsNullOrWhiteSpace(billingPeriod))
            filtered = filtered.Where(b => b.BillingPeriod == billingPeriod);

        var byFlat = filtered
            .GroupBy(b => new { b.Flat?.FlatNumber, b.FlatId })
            .Select(g => new RevenueByFlatDto(
                g.Key.FlatNumber ?? "Unknown",
                g.Sum(b => b.GrandTotal),
                g.Sum(b => b.AmountPaid),
                g.Sum(b => b.GrandTotal - b.AmountPaid)))
            .ToList();

        var totalBilled = byFlat.Sum(x => x.Billed);
        var totalCollected = byFlat.Sum(x => x.Collected);
        var totalOutstanding = byFlat.Sum(x => x.Outstanding);

        return new RevenueReportDto(totalBilled, totalCollected, totalOutstanding, byFlat);
    }

    public async Task<OutstandingReportDto?> GetOutstandingReportAsync()
    {
        var tenantId = _currentUser.TenantId;
        if (tenantId == null) return null;

        var bills = await _billRepository.GetAllWithDetailsAsync();
        var filtered = bills
            .Where(b => b.TenantId == tenantId && !b.IsDeleted && b.Status != "Cancelled" && b.BalanceOutstanding > 0)
            .OrderBy(b => b.DueDate)
            .ToList();

        var flats = filtered.Select(b => new OutstandingByFlatDto(
            b.Flat?.FlatNumber ?? "Unknown",
            b.Flat?.Members?.FirstOrDefault(m => m.IsPrimary) != null
                ? $"{b.Flat.Members.First(m => m.IsPrimary).FirstName} {b.Flat.Members.First(m => m.IsPrimary).LastName}"
                : null,
            b.BalanceOutstanding,
            b.DueDate < DateTime.UtcNow ? (int)(DateTime.UtcNow - b.DueDate).TotalDays : 0))
            .ToList();

        var grandTotal = flats.Sum(f => f.Amount);

        return new OutstandingReportDto(flats, grandTotal);
    }
}
