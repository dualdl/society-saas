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
public class ChargesController : ControllerBase
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public ChargesController(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var tenantId = _currentUser.TenantId;
        if (tenantId == null) return BadRequest("Tenant not selected");

        var charges = await _context.Charges
            .Where(c => c.TenantId == tenantId && !c.IsDeleted)
            .OrderBy(c => c.Name)
            .Select(c => new ChargeDto(c.Id, c.Name, c.Description, c.CalculationType, c.Amount, c.IsRecurring, c.IsActive))
            .ToListAsync();
        return Ok(charges);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var charge = await _context.Charges.FindAsync(id);
        if (charge == null) return NotFound();
        return Ok(new ChargeDto(charge.Id, charge.Name, charge.Description, charge.CalculationType, charge.Amount, charge.IsRecurring, charge.IsActive));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateChargeRequest request)
    {
        var tenantId = _currentUser.TenantId;
        if (tenantId == null) return BadRequest("Tenant not selected");

        var charge = new Charge
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId.Value,
            Name = request.Name,
            Description = request.Description,
            CalculationType = request.CalculationType,
            Amount = request.Amount,
            IsRecurring = request.IsRecurring,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Charges.Add(charge);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = charge.Id }, charge);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateChargeRequest request)
    {
        var charge = await _context.Charges.FindAsync(id);
        if (charge == null) return NotFound();

        charge.Name = request.Name;
        charge.Description = request.Description;
        charge.CalculationType = request.CalculationType;
        charge.Amount = request.Amount;
        charge.IsRecurring = request.IsRecurring;
        charge.IsActive = request.IsActive;
        charge.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return Ok(charge);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var charge = await _context.Charges.FindAsync(id);
        if (charge == null) return NotFound();

        charge.IsDeleted = true;
        charge.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
