using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocietySaaS.Application.Common.DTOs;
using SocietySaaS.Application.Common.Interfaces;
using SocietySaaS.Domain.Entities;
using SocietySaaS.Infrastructure.Services;

namespace SocietySaaS.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Roles = "SuperAdmin")]
public class SuperAdminController : ControllerBase
{
    private readonly IApplicationDbContext _context;
    private readonly JwtTokenService _jwtTokenService;

    public SuperAdminController(IApplicationDbContext context, JwtTokenService jwtTokenService)
    {
        _context = context;
        _jwtTokenService = jwtTokenService;
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard()
    {
        var totalSocieties = await _context.Tenants.CountAsync(t => !t.IsDeleted);
        var activeSocieties = await _context.Tenants.CountAsync(t => !t.IsDeleted && t.IsActive);
        var totalUsers = await _context.Users.CountAsync(u => !u.IsSuperAdmin);

        var recentSocieties = await _context.Tenants
            .Where(t => !t.IsDeleted)
            .OrderByDescending(t => t.CreatedAt)
            .Take(10)
            .Select(t => new SocietySummaryDto(
                t.Id, t.Name,
                t.Flats.Count(f => !f.IsDeleted),
                0,
                t.CreatedAt))
            .ToListAsync();

        return Ok(new AdminDashboardDto(totalSocieties, activeSocieties, totalUsers, recentSocieties));
    }

    [HttpGet("societies")]
    public async Task<IActionResult> GetAllSocieties()
    {
        var societies = await _context.Tenants
            .Where(t => !t.IsDeleted)
            .OrderBy(t => t.Name)
            .Select(t => new TenantDto(t.Id, t.Name, t.Address, t.City, t.State, t.PinCode, t.Phone, t.Email, t.IsActive, t.CreatedAt))
            .ToListAsync();
        return Ok(societies);
    }

    [HttpPost("societies")]
    public async Task<IActionResult> CreateSociety([FromBody] CreateTenantRequest request)
    {
        var tenant = new Tenant
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Address = request.Address,
            City = request.City,
            State = request.State,
            PinCode = request.PinCode,
            Phone = request.Phone,
            Email = request.Email,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Tenants.Add(tenant);

        // Create default admin user for this society
        var adminUser = new User
        {
            Id = Guid.NewGuid(),
            Email = $"admin@{request.Name.ToLower().Replace(" ", "")}.com",
            FirstName = "Admin",
            LastName = request.Name,
            PasswordHash = JwtTokenService.HashPassword("Admin@123"),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        _context.Users.Add(adminUser);

        var userTenant = new UserTenant
        {
            Id = Guid.NewGuid(),
            UserId = adminUser.Id,
            TenantId = tenant.Id,
            Role = "Admin",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        _context.UserTenants.Add(userTenant);

        // Create default charges
        var defaultCharges = new[]
        {
            new Charge { Id = Guid.NewGuid(), TenantId = tenant.Id, Name = "Maintenance", CalculationType = "Fixed", Amount = 3000, IsRecurring = true, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Charge { Id = Guid.NewGuid(), TenantId = tenant.Id, Name = "Sinking Fund", CalculationType = "Fixed", Amount = 500, IsRecurring = true, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Charge { Id = Guid.NewGuid(), TenantId = tenant.Id, Name = "Water Charges", CalculationType = "Fixed", Amount = 400, IsRecurring = true, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Charge { Id = Guid.NewGuid(), TenantId = tenant.Id, Name = "Parking", CalculationType = "Fixed", Amount = 1000, IsRecurring = true, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Charge { Id = Guid.NewGuid(), TenantId = tenant.Id, Name = "Electricity Common Area", CalculationType = "Fixed", Amount = 600, IsRecurring = true, IsActive = true, CreatedAt = DateTime.UtcNow },
        };
        _context.Charges.AddRange(defaultCharges);

        await _context.SaveChangesAsync();

        return CreatedAtAction(null, new { id = tenant.Id }, new
        {
            tenant.Id, tenant.Name,
            AdminEmail = adminUser.Email,
            AdminPassword = "Admin@123"
        });
    }

    [HttpPut("societies/{id:guid}")]
    public async Task<IActionResult> UpdateSociety(Guid id, [FromBody] UpdateTenantRequest request)
    {
        var tenant = await _context.Tenants.FindAsync(id);
        if (tenant == null) return NotFound();

        tenant.Name = request.Name;
        tenant.Address = request.Address;
        tenant.City = request.City;
        tenant.State = request.State;
        tenant.PinCode = request.PinCode;
        tenant.Phone = request.Phone;
        tenant.Email = request.Email;
        tenant.IsActive = request.IsActive;
        tenant.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return Ok(tenant);
    }

    [HttpDelete("societies/{id:guid}")]
    public async Task<IActionResult> DeleteSociety(Guid id)
    {
        var tenant = await _context.Tenants.FindAsync(id);
        if (tenant == null) return NotFound();

        tenant.IsDeleted = true;
        tenant.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
