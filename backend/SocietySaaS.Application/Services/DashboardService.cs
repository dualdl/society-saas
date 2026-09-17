using SocietySaaS.Application.Common.DTOs;
using SocietySaaS.Application.Common.Interfaces;
using SocietySaaS.Domain.Interfaces;

namespace SocietySaaS.Application.Services;

public interface IDashboardService
{
    Task<DashboardDto?> GetDashboardAsync();
    Task<AdminDashboardDto?> GetAdminDashboardAsync();
}

public class DashboardService : IDashboardService
{
    private readonly IFlatRepository _flatRepository;
    private readonly IMemberRepository _memberRepository;
    private readonly IBillRepository _billRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly ITenantRepository _tenantRepository;
    private readonly ICurrentUserService _currentUser;

    public DashboardService(
        IFlatRepository flatRepository,
        IMemberRepository memberRepository,
        IBillRepository billRepository,
        IPaymentRepository paymentRepository,
        ITenantRepository tenantRepository,
        ICurrentUserService currentUser)
    {
        _flatRepository = flatRepository;
        _memberRepository = memberRepository;
        _billRepository = billRepository;
        _paymentRepository = paymentRepository;
        _tenantRepository = tenantRepository;
        _currentUser = currentUser;
    }

    public async Task<DashboardDto?> GetDashboardAsync()
    {
        var tenantId = _currentUser.TenantId;
        if (tenantId == null) return null;

        var flats = await _flatRepository.GetAllWithMembersAsync();
        var members = await _memberRepository.GetAllWithFlatAsync();
        var allBills = await _billRepository.GetAllWithDetailsAsync();
        var allPayments = await _paymentRepository.GetAllWithDetailsAsync();

        var tenantFlats = flats.Where(f => f.TenantId == tenantId && !f.IsDeleted).ToList();
        var totalFlats = tenantFlats.Count;
        var occupiedFlats = tenantFlats.Count(f => f.OccupancyStatus != "Vacant");
        var totalMembers = members.Count(m => m.TenantId == tenantId && !m.IsDeleted && m.IsActive);

        var tenantBills = allBills.Where(b => b.TenantId == tenantId && !b.IsDeleted && b.Status != "Cancelled").ToList();
        var totalBilled = tenantBills.Sum(b => b.GrandTotal);
        var totalCollected = tenantBills.Sum(b => b.AmountPaid);
        var totalOutstanding = totalBilled - totalCollected;
        var collectionPct = totalBilled > 0 ? Math.Round(totalCollected / totalBilled * 100, 1) : 0;

        var monthlyTrends = tenantBills
            .GroupBy(b => b.BillingPeriod)
            .OrderByDescending(g => g.Key)
            .Take(6)
            .Select(g => new MonthlyTrendDto(g.Key, g.Sum(b => b.GrandTotal), g.Sum(b => b.AmountPaid)))
            .ToList();

        var recentPayments = allPayments
            .Where(p => p.TenantId == tenantId && !p.IsDeleted)
            .OrderByDescending(p => p.PaymentDate)
            .Take(5)
            .Select(p => new RecentActivityDto(
                $"Payment of \u20b9{p.Amount:N0} received from Flat {p.Flat?.FlatNumber}",
                "Payment", p.PaymentDate))
            .ToList();

        var recentBills = tenantBills
            .OrderByDescending(b => b.BillDate)
            .Take(5)
            .Select(b => new RecentActivityDto(
                $"Bill {b.BillNumber} generated for Flat {b.Flat?.FlatNumber} - \u20b9{b.GrandTotal:N0}",
                "Bill", b.BillDate))
            .ToList();

        var recentActivities = recentPayments.Concat(recentBills)
            .OrderByDescending(a => a.Timestamp)
            .Take(10)
            .ToList();

        return new DashboardDto(
            totalFlats, occupiedFlats, totalMembers,
            totalBilled, totalCollected, totalOutstanding,
            collectionPct, monthlyTrends, recentActivities);
    }

    public async Task<AdminDashboardDto?> GetAdminDashboardAsync()
    {
        var tenants = await _tenantRepository.GetAllAsync();
        var allFlats = await _flatRepository.GetAllWithMembersAsync();

        var nonDeletedTenants = tenants.Where(t => !t.IsDeleted).ToList();
        var totalSocieties = nonDeletedTenants.Count;
        var activeSocieties = nonDeletedTenants.Count(t => t.IsActive);

        var recentSocieties = nonDeletedTenants
            .OrderByDescending(t => t.CreatedAt)
            .Take(10)
            .Select(t => new SocietySummaryDto(
                t.Id, t.Name,
                allFlats.Count(f => f.TenantId == t.Id && !f.IsDeleted),
                0,
                t.CreatedAt))
            .ToList();

        return new AdminDashboardDto(totalSocieties, activeSocieties, 0, recentSocieties);
    }
}
