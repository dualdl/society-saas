namespace SocietySaaS.Application.Common.DTOs;

public record ChargeDto(Guid Id, string Name, string? Description, string CalculationType, decimal Amount, bool IsRecurring, bool IsActive);
public record CreateChargeRequest(string Name, string? Description, string CalculationType, decimal Amount, bool IsRecurring);
public record UpdateChargeRequest(string Name, string? Description, string CalculationType, decimal Amount, bool IsRecurring, bool IsActive);
