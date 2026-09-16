using Microsoft.AspNetCore.Mvc;
using SocietySaaS.Application.Common.Interfaces;
using SocietySaaS.Domain.Entities;

namespace SocietySaaS.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class HealthController : ControllerBase
{
    private readonly IApplicationDbContext _context;
    
    public HealthController(IApplicationDbContext context)
    {
        _context = context;
    }
    
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            Status = "Healthy",
            Timestamp = DateTime.UtcNow,
            Version = "1.0.0"
        });
    }
    
    [HttpGet("db")]
    public async Task<IActionResult> CheckDatabase()
    {
        try
        {
            await _context.SaveChangesAsync();
            return Ok(new { Status = "Database Connected" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Status = "Database Error", Error = ex.Message });
        }
    }
}
