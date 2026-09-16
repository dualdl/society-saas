namespace SocietySaaS.Application.Common.DTOs;

public record LoginRequest(string Email, string Password);
public record LoginResponse(string Token, string Email, string FirstName, string LastName, bool IsSuperAdmin, Guid? TenantId, string? TenantName);
public record RegisterRequest(string Email, string Password, string FirstName, string LastName, string? Mobile);
public record ChangePasswordRequest(string OldPassword, string NewPassword);
