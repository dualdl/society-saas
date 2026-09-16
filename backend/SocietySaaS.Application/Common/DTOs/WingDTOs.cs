namespace SocietySaaS.Application.Common.DTOs;

public record WingDto(Guid Id, string Name, int TotalFloors, int FlatsPerFloor, bool IsActive);
public record CreateWingRequest(string Name, int TotalFloors, int FlatsPerFloor);
public record UpdateWingRequest(string Name, int TotalFloors, int FlatsPerFloor, bool IsActive);
