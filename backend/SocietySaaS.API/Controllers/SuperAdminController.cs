using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

    [HttpPost("societies/import")]
    public async Task<IActionResult> ImportSocieties(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
                return BadRequest(ApiResponse<object>.Fail("No file uploaded"));

            using var stream = new StreamReader(file.OpenReadStream());
            var content = await stream.ReadToEndAsync();
            var societies = JsonSerializer.Deserialize<List<CreateTenantRequest>>(content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (societies == null || societies.Count == 0)
                return BadRequest(ApiResponse<object>.Fail("No valid societies in file"));

            var created = new List<object>();
            foreach (var s in societies)
            {
                try
                {
                    var result = await _tenantService.CreateAsync(s);
                    var tenant = (dynamic)result;

                    var adminUser = new User
                    {
                        Id = Guid.NewGuid(),
                        Email = $"admin@{s.Name.ToLower().Replace(" ", "")}.com",
                        FirstName = "Admin",
                        LastName = s.Name,
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

                    created.Add(new { tenant.Id, tenant.Name, AdminEmail = adminUser.Email });
                }
                catch { }
            }

            await _context.SaveChangesAsync();
            return Ok(ApiResponse<object>.Ok(new { imported = created.Count, societies = created }));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.Fail(ex.Message));
        }
    }

    [HttpGet("societies/{id:guid}/backup")]
    public async Task<IActionResult> BackupSociety(Guid id)
    {
        try
        {
            var tenant = await _context.Tenants.FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
            if (tenant == null) return NotFound(ApiResponse<object>.Fail("Society not found"));

            var backup = new
            {
                Society = new { tenant.Id, tenant.Name, tenant.Address, tenant.City, tenant.State, tenant.PinCode, tenant.Phone, tenant.Email, tenant.CreatedAt },
                Wings = await _context.Wings.Where(w => w.TenantId == id && !w.IsDeleted)
                    .Select(w => new { w.Id, w.Name, w.TotalFloors, w.FlatsPerFloor }).ToListAsync(),
                Flats = await _context.Flats.Where(f => f.TenantId == id && !f.IsDeleted)
                    .Select(f => new { f.Id, f.FlatNumber, f.Floor, f.CarpetArea, f.BuiltUpArea, f.FlatType, f.OccupancyStatus, f.WingId }).ToListAsync(),
                Members = await _context.Members.Where(m => m.TenantId == id && !m.IsDeleted)
                    .Select(m => new { m.Id, m.FirstName, m.LastName, m.Mobile, m.Email, m.MemberType, m.IsPrimary, m.FlatId }).ToListAsync(),
                Charges = await _context.Charges.Where(c => c.TenantId == id && !c.IsDeleted)
                    .Select(c => new { c.Id, c.Name, c.CalculationType, c.Amount, c.IsRecurring }).ToListAsync(),
                Bills = await _context.Bills.Where(b => b.TenantId == id && !b.IsDeleted)
                    .Select(b => new { b.Id, b.BillNumber, b.FlatId, b.GrandTotal, b.BalanceOutstanding, b.Status, b.BillingPeriod }).ToListAsync(),
                Payments = await _context.Payments.Where(p => p.TenantId == id && !p.IsDeleted)
                    .Select(p => new { p.Id, p.PaymentNumber, p.FlatId, p.Amount, p.PaymentMode, p.PaymentDate, p.Status }).ToListAsync(),
                BackupDate = DateTime.UtcNow
            };

            var json = JsonSerializer.Serialize(backup, new JsonSerializerOptions { WriteIndented = true });
            var bytes = Encoding.UTF8.GetBytes(json);
            return File(bytes, "application/json", $"society-backup-{tenant.Name}-{DateTime.UtcNow:yyyyMMdd}.json");
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
