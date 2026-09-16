namespace SocietySaaS.Application.Common.DTOs;

public record RevenueReportDto(decimal TotalBilled, decimal TotalCollected, decimal TotalOutstanding, List<RevenueByFlatDto> ByFlat);
public record RevenueByFlatDto(string FlatNumber, decimal Billed, decimal Collected, decimal Outstanding);
public record OutstandingReportDto(List<OutstandingByFlatDto> Flats, decimal GrandTotal);
public record OutstandingByFlatDto(string FlatNumber, string? MemberName, decimal Amount, int DaysOverdue);
