using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocietySaaS.Infrastructure.Persistence;
using SocietySaaS.Shared;

namespace SocietySaaS.API.Controllers;

[ApiController]
[Route("api/v1/admin")]
public class SeedController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public SeedController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpPost("seed")]
    public async Task<IActionResult> Seed()
    {
        try
        {
            if (!await _db.Users.AnyAsync(u => u.IsSuperAdmin))
            {
                await SeedData.SeedAsync(_db);
                return Ok(ApiResponse<object>.Ok(new { message = "Database seeded successfully" }));
            }
            return Ok(ApiResponse<object>.Ok(new { message = "Database already seeded. Use POST /api/v1/admin/seed/force to re-seed." }));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.Fail(ex.Message, ex.InnerException != null ? new List<SocietySaaS.Shared.ApiError> { new SocietySaaS.Shared.ApiError { Code = "INNER_EXCEPTION", Message = ex.InnerException.Message } } : null));
        }
    }

    [HttpPost("seed/force")]
    public async Task<IActionResult> ForceSeed()
    {
        try
        {
            await SeedData.SeedAsync(_db);
            return Ok(ApiResponse<object>.Ok(new { message = "Database re-seeded successfully" }));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.Fail(ex.Message, ex.InnerException != null ? new List<SocietySaaS.Shared.ApiError> { new SocietySaaS.Shared.ApiError { Code = "INNER_EXCEPTION", Message = ex.InnerException.Message } } : null));
        }
    }

    [HttpPost("seed/recreate")]
    public async Task<IActionResult> RecreateAndSeed()
    {
        try
        {
            await _db.Database.EnsureDeletedAsync();
            await _db.Database.MigrateAsync();
            await SeedData.SeedAsync(_db);
            return Ok(ApiResponse<object>.Ok(new { message = "Database recreated, migrated, and seeded successfully" }));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.Fail(ex.Message, ex.InnerException != null ? new List<SocietySaaS.Shared.ApiError> { new SocietySaaS.Shared.ApiError { Code = "INNER_EXCEPTION", Message = ex.InnerException.Message } } : null));
        }
    }

    [HttpGet("status")]
    public async Task<IActionResult> Status()
    {
        try
        {
            var canConnect = await _db.Database.CanConnectAsync();
            if (!canConnect) return Ok(ApiResponse<object>.Ok(new { connected = false }));

            var userCount = await _db.Users.CountAsync();
            var tenantCount = await _db.Tenants.CountAsync();
            var flatCount = await _db.Flats.CountAsync();
            var billCount = await _db.Bills.CountAsync();
            var paymentCount = await _db.Payments.CountAsync();
            var receiptCount = await _db.Receipts.CountAsync();
            return Ok(ApiResponse<object>.Ok(new { connected = true, userCount, tenantCount, flatCount, billCount, paymentCount, receiptCount }));
        }
        catch (Exception ex)
        {
            return Ok(ApiResponse<object>.Ok(new { connected = false, error = ex.Message }));
        }
    }
}
