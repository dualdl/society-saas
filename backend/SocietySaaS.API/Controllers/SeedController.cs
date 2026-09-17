using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocietySaaS.Infrastructure.Persistence;

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
                return Ok(new { message = "Database seeded successfully" });
            }
            return Ok(new { message = "Database already seeded" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message, inner = ex.InnerException?.Message });
        }
    }

    [HttpGet("status")]
    public async Task<IActionResult> Status()
    {
        try
        {
            var canConnect = await _db.Database.CanConnectAsync();
            if (!canConnect) return Ok(new { connected = false });

            var userCount = await _db.Users.CountAsync();
            var tenantCount = await _db.Tenants.CountAsync();
            var flatCount = await _db.Flats.CountAsync();
            var billCount = await _db.Bills.CountAsync();
            return Ok(new { connected = true, userCount, tenantCount, flatCount, billCount });
        }
        catch (Exception ex)
        {
            return Ok(new { connected = false, error = ex.Message });
        }
    }
}
