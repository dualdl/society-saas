namespace SocietySaaS.Application.Common.DTOs;

public record FlatDto(Guid Id, string FlatNumber, int Floor, decimal CarpetArea, decimal BuiltUpArea, string? FlatType, string OccupancyStatus, bool IsActive, Guid? WingId, string? WingName, int MemberCount, decimal BalanceOutstanding);
public record CreateFlatRequest(string FlatNumber, int Floor, decimal CarpetArea, decimal BuiltUpArea, string? FlatType, string OccupancyStatus, Guid? WingId);
public record UpdateFlatRequest(string FlatNumber, int Floor, decimal CarpetArea, decimal BuiltUpArea, string? FlatType, string OccupancyStatus, bool IsActive, Guid? WingId);
