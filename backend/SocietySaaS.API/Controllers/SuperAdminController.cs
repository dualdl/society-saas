using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocietySaaS.Application.Common.DTOs;
using SocietySaaS.Application.Common.Interfaces;
using SocietySaaS.Application.Services;
using SocietySaaS.Domain.Entities;
using SocietySaaS.Infrastructure.Services;
using SocietySaaS.Shared;

namespace SocietySaaS.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Roles = "SuperAdmin")]
public class SuperAdminController : ControllerBase
{
    private readonly ITenantService _tenantService;
    private readonly IDashboardService _dashboardService;
    private readonly IApplicationDbContext _context;
    private readonly JwtTokenService _jwtTokenService;

    public SuperAdminController(ITenantService tenantService, IDashboardService dashboardService, IApplicationDbContext context, JwtTokenService jwtTokenService)
    {
        _tenantService = tenantService;
        _dashboardService = dashboardService;
        _context = context;
        _jwtTokenService = jwtTokenService;
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard()
    {
        try
        {
            var dashboard = await _dashboardService.GetAdminDashboardAsync();
            if (dashboard == null) return NotFound(ApiResponse<object>.Fail("Dashboard data not available"));
            return Ok(ApiResponse<AdminDashboardDto>.Ok(dashboard));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.Fail(ex.Message));
        }
    }

    [HttpGet("societies")]
    public async Task<IActionResult> GetAllSocieties()
    {
        try
        {
            var societies = await _tenantService.GetAllAsync();
            return Ok(ApiResponse<List<TenantDto>>.Ok(societies));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.Fail(ex.Message));
        }
    }

    [HttpPost("societies")]
    public async Task<IActionResult> CreateSociety([FromBody] CreateTenantRequest request)
    {
        try
        {
            var result = await _tenantService.CreateAsync(request);

            var tenant = (dynamic)result;

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

            return CreatedAtAction(null, new { id = tenant.Id }, ApiResponse<object>.Ok(new
            {
                tenant.Id, tenant.Name,
                AdminEmail = adminUser.Email,
                AdminPassword = "Admin@123"
            }));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.Fail(ex.Message));
        }
    }

    [HttpPut("societies/{id:guid}")]
    public async Task<IActionResult> UpdateSociety(Guid id, [FromBody] UpdateTenantRequest request)
    {
        try
        {
            var tenant = await _tenantService.UpdateAsync(id, request);
            return Ok(ApiResponse<TenantDto>.Ok(tenant));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<object>.Fail(ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.Fail(ex.Message));
        }
    }

    [HttpDelete("societies/{id:guid}")]
    public async Task<IActionResult> DeleteSociety(Guid id)
    {
        try
        {
            await _tenantService.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<object>.Fail(ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.Fail(ex.Message));
        }
    }
}
