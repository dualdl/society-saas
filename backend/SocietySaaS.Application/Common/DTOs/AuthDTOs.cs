namespace SocietySaaS.Application.Common.DTOs;

public record LoginRequest(string Email, string Password);

public class LoginResponse
{
    public bool Success { get; set; }
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool RequiresOtp { get; set; }
    public UserDto? User { get; set; }
}

public record RegisterRequest(string Email, string Password, string FirstName, string LastName, string? Mobile);
public record ChangePasswordRequest(string OldPassword, string NewPassword);

public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Mobile { get; set; }
    public bool IsSuperAdmin { get; set; }
    public Guid? TenantId { get; set; }
}
