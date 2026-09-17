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
            await _db.Database.EnsureDeletedAsync();
            await _db.Database.EnsureCreatedAsync();

            if (!await _db.Users.AnyAsync(u => u.IsSuperAdmin))
            {
                await SeedData.SeedAsync(_db);
                return Ok(new { message = "Database created and seeded successfully" });
            }
            return Ok(new { message = "Database already seeded" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message, inner = ex.InnerException?.Message });
        }
    }

    [HttpGet("status")]
    public async Task<IActionResult> Status()
    {
        try
        {
            var userCount = await _db.Users.CountAsync();
            var flatCount = await _db.Flats.CountAsync();
            var billCount = await _db.Bills.CountAsync();
            return Ok(new { userCount, flatCount, billCount, connected = true });
        }
        catch (Exception ex)
        {
            return Ok(new { connected = false, error = ex.Message });
        }
    }
}
