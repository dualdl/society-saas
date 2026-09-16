namespace SocietySaaS.Application.Common.DTOs;

public record TenantDto(Guid Id, string Name, string? Address, string? City, string? State, string? PinCode, string? Phone, string? Email, bool IsActive, DateTime CreatedAt);
public record CreateTenantRequest(string Name, string? Address, string? City, string? State, string? PinCode, string? Phone, string? Email);
public record UpdateTenantRequest(string Name, string? Address, string? City, string? State, string? PinCode, string? Phone, string? Email, bool IsActive);
