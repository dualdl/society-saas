using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using SocietySaaS.Application.Common.DTOs;
using SocietySaaS.Application.Common.Interfaces;
using SocietySaaS.Domain.Entities;

namespace SocietySaaS.Application.Services;

public interface IAuthService
{
    Task<LoginResponse> RequestOtpAsync(string email);
    Task<LoginResponse> VerifyOtpAsync(string email, string code);
    Task<LoginResponse> LoginAsync(LoginRequest request);
    Task<LoginResponse> RefreshTokenAsync(string refreshToken);
    Task<LoginResponse> RegisterAsync(RegisterRequest request);
    Task<LoginResponse> ChangePasswordAsync(string userId, ChangePasswordRequest request);
    Task<UserDto?> GetMeAsync(string userId);
}

public class AuthService : IAuthService
{
    private readonly IApplicationDbContext _context;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ICurrentUserService _currentUser;

    public AuthService(IApplicationDbContext context, IJwtTokenService jwtTokenService, ICurrentUserService currentUser)
    {
        _context = context;
        _jwtTokenService = jwtTokenService;
        _currentUser = currentUser;
    }

    public async Task<LoginResponse> RequestOtpAsync(string email)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.IsActive);
        if (user == null)
            return new LoginResponse { Success = false, Message = "No account found with this email" };

        var recentOtps = await _context.OtpRequests
            .CountAsync(o => o.Target == email && o.CreatedAt > DateTime.UtcNow.AddMinutes(-10));
        if (recentOtps >= 3)
            return new LoginResponse { Success = false, Message = "Too many OTP requests. Please wait 10 minutes." };

        var code = new Random().Next(100000, 999999).ToString();
        var otp = new OtpRequest
        {
            Target = email,
            Code = code,
            Purpose = "Login",
            ExpiresAt = DateTime.UtcNow.AddMinutes(5),
            IsUsed = false
        };
        await _context.OtpRequests.AddAsync(otp);
        await _context.SaveChangesAsync();

        return new LoginResponse
        {
            Success = true,
            Message = $"OTP sent to {email}. Code: {code}",
            RequiresOtp = true
        };
    }

    public async Task<LoginResponse> VerifyOtpAsync(string email, string code)
    {
        var otp = await _context.OtpRequests
            .Where(o => o.Target == email && o.Code == code && !o.IsUsed && o.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefaultAsync();

        if (otp == null)
            return new LoginResponse { Success = false, Message = "Invalid or expired OTP" };

        if (otp.AttemptCount >= 5)
            return new LoginResponse { Success = false, Message = "Too many failed attempts. Request a new OTP." };

        otp.AttemptCount++;
        otp.IsUsed = true;
        await _context.SaveChangesAsync();

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.IsActive);
        if (user == null)
            return new LoginResponse { Success = false, Message = "User not found" };

        user.LastLoginAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return await GenerateTokensAsync(user);
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email && u.IsActive);
        if (user == null)
            return new LoginResponse { Success = false, Message = "Invalid credentials" };

        if (string.IsNullOrEmpty(user.PasswordHash))
            return new LoginResponse { Success = false, Message = "Invalid credentials" };

        var isValid = VerifyPassword(request.Password, user.PasswordHash);
        if (!isValid)
            return new LoginResponse { Success = false, Message = "Invalid credentials" };

        // Migrate SHA256 hash to BCrypt on successful login
        if (!IsBcryptHash(user.PasswordHash))
        {
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            await _context.SaveChangesAsync();
        }

        user.LastLoginAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return await GenerateTokensAsync(user);
    }

    public async Task<LoginResponse> RefreshTokenAsync(string refreshToken)
    {
        var token = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken && !rt.IsRevoked && rt.ExpiresAt > DateTime.UtcNow);
        if (token == null)
            return new LoginResponse { Success = false, Message = "Invalid refresh token" };

        var user = await _context.Users.FindAsync(token.UserId);
        if (user == null || !user.IsActive)
            return new LoginResponse { Success = false, Message = "User not found" };

        token.IsRevoked = true;
        await _context.SaveChangesAsync();

        return await GenerateTokensAsync(user);
    }

    public async Task<LoginResponse> RegisterAsync(RegisterRequest request)
    {
        if (await _context.Users.AnyAsync(u => u.Email == request.Email && u.IsActive))
            return new LoginResponse { Success = false, Message = "Email already registered" };

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Mobile = request.Mobile,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        return await GenerateTokensAsync(user);
    }

    public async Task<LoginResponse> ChangePasswordAsync(string userId, ChangePasswordRequest request)
    {
        var user = await _context.Users.FindAsync(Guid.Parse(userId));
        if (user == null) return new LoginResponse { Success = false, Message = "User not found" };

        if (string.IsNullOrEmpty(user.PasswordHash) || !BCrypt.Net.BCrypt.Verify(request.OldPassword, user.PasswordHash))
            return new LoginResponse { Success = false, Message = "Current password is incorrect" };

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        await _context.SaveChangesAsync();

        return new LoginResponse { Success = true, Message = "Password changed successfully" };
    }

    public async Task<UserDto?> GetMeAsync(string userId)
    {
        var user = await _context.Users.FindAsync(Guid.Parse(userId));
        if (user == null) return null;

        var tenantId = _currentUser.TenantId;
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Mobile = user.Mobile,
            IsSuperAdmin = user.IsSuperAdmin,
            TenantId = tenantId
        };
    }

    private async Task<LoginResponse> GenerateTokensAsync(User user)
    {
        var tenantId = _currentUser.TenantId;
        var token = _jwtTokenService.GenerateToken(user, tenantId);
        var refreshToken = Guid.NewGuid().ToString();

        await _context.RefreshTokens.AddAsync(new RefreshToken
        {
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        });
        await _context.SaveChangesAsync();

        return new LoginResponse
        {
            Success = true,
            Token = token,
            RefreshToken = refreshToken,
            Message = "Login successful",
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Mobile = user.Mobile,
                IsSuperAdmin = user.IsSuperAdmin,
                TenantId = tenantId
            }
        };
    }

    private static bool IsBcryptHash(string hash)
    {
        return hash.StartsWith("$2a$") || hash.StartsWith("$2b$") || hash.StartsWith("$2y$");
    }

    private static bool VerifyPassword(string password, string hash)
    {
        if (IsBcryptHash(hash))
            return BCrypt.Net.BCrypt.Verify(password, hash);

        // Legacy SHA256 verification for backward compatibility
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + "SocietySaaS_Salt_2024!"));
        var computedHash = Convert.ToBase64String(bytes);
        return computedHash == hash;
    }

}
