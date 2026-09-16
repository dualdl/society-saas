using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocietySaaS.Application.Common.DTOs;
using SocietySaaS.Application.Common.Interfaces;
using SocietySaaS.Domain.Entities;

namespace SocietySaaS.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class WingsController : ControllerBase
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public WingsController(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var tenantId = _currentUser.TenantId;
        if (tenantId == null) return BadRequest("Tenant not selected");

        var wings = await _context.Wings
            .Where(w => w.TenantId == tenantId && !w.IsDeleted)
            .OrderBy(w => w.Name)
            .Select(w => new WingDto(w.Id, w.Name, w.TotalFloors, w.FlatsPerFloor, w.IsActive))
            .ToListAsync();
        return Ok(wings);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var wing = await _context.Wings.FindAsync(id);
        if (wing == null) return NotFound();
        return Ok(new WingDto(wing.Id, wing.Name, wing.TotalFloors, wing.FlatsPerFloor, wing.IsActive));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateWingRequest request)
    {
        var tenantId = _currentUser.TenantId;
        if (tenantId == null) return BadRequest("Tenant not selected");

        var wing = new Wing
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId.Value,
            Name = request.Name,
            TotalFloors = request.TotalFloors,
            FlatsPerFloor = request.FlatsPerFloor,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Wings.Add(wing);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = wing.Id }, wing);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateWingRequest request)
    {
        var wing = await _context.Wings.FindAsync(id);
        if (wing == null) return NotFound();

        wing.Name = request.Name;
        wing.TotalFloors = request.TotalFloors;
        wing.FlatsPerFloor = request.FlatsPerFloor;
        wing.IsActive = request.IsActive;
        wing.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return Ok(wing);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var wing = await _context.Wings.FindAsync(id);
        if (wing == null) return NotFound();

        wing.IsDeleted = true;
        wing.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
