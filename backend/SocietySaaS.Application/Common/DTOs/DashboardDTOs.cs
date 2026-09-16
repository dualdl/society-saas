namespace SocietySaaS.Application.Common.DTOs;

public record DashboardDto(
    int TotalFlats, int OccupiedFlats, int TotalMembers,
    decimal TotalBilled, decimal TotalCollected, decimal TotalOutstanding,
    decimal CollectionPercentage,
    List<MonthlyTrendDto> MonthlyTrends,
    List<RecentActivityDto> RecentActivities);

public record MonthlyTrendDto(string Month, decimal Billed, decimal Collected);
public record RecentActivityDto(string Description, string Type, DateTime Timestamp);

public record AdminDashboardDto(
    int TotalSocieties, int ActiveSocieties, int TotalUsers,
    List<SocietySummaryDto> RecentSocieties);

public record SocietySummaryDto(Guid Id, string Name, int FlatCount, decimal CollectionRate, DateTime CreatedAt);
