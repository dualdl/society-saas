using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocietySaaS.Application.Common.DTOs;
using SocietySaaS.Application.Common.Interfaces;
using SocietySaaS.Domain.Entities;
using SocietySaaS.Infrastructure.Services;

namespace SocietySaaS.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IApplicationDbContext _context;
    private readonly JwtTokenService _jwtTokenService;
    private readonly ICurrentUserService _currentUser;

    public AuthController(IApplicationDbContext context, JwtTokenService jwtTokenService, ICurrentUserService currentUser)
    {
        _context = context;
        _jwtTokenService = jwtTokenService;
        _currentUser = currentUser;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email && u.IsActive);
        if (user == null || !JwtTokenService.VerifyPassword(request.Password, user.PasswordHash ?? ""))
            return Unauthorized(new { message = "Invalid email or password" });

        user.LastLoginAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        Guid? tenantId = null;
        string? tenantName = null;

        if (!user.IsSuperAdmin)
        {
            var userTenant = await _context.UserTenants
                .Include(ut => ut.Tenant)
                .FirstOrDefaultAsync(ut => ut.UserId == user.Id && ut.IsActive);
            if (userTenant != null)
            {
                tenantId = userTenant.TenantId;
                tenantName = userTenant.Tenant?.Name;
            }
        }

        var token = _jwtTokenService.GenerateToken(user, tenantId);

        return Ok(new LoginResponse(token, user.Email, user.FirstName ?? "", user.LastName ?? "", user.IsSuperAdmin, tenantId, tenantName));
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            return Conflict(new { message = "Email already registered" });

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Mobile = request.Mobile,
            PasswordHash = JwtTokenService.HashPassword(request.Password),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var token = _jwtTokenService.GenerateToken(user);
        return Ok(new LoginResponse(token, user.Email, user.FirstName ?? "", user.LastName ?? "", false, null, null));
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        if (!_currentUser.IsAuthenticated)
            return Unauthorized();

        var user = await _context.Users.FindAsync(_currentUser.UserId);
        if (user == null) return NotFound();

        return Ok(new
        {
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName,
            user.IsSuperAdmin,
            _currentUser.TenantId
        });
    }

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        if (!_currentUser.IsAuthenticated) return Unauthorized();

        var user = await _context.Users.FindAsync(_currentUser.UserId);
        if (user == null) return NotFound();

        if (!JwtTokenService.VerifyPassword(request.OldPassword, user.PasswordHash ?? ""))
            return BadRequest(new { message = "Current password is incorrect" });

        user.PasswordHash = JwtTokenService.HashPassword(request.NewPassword);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Password changed successfully" });
    }
}
