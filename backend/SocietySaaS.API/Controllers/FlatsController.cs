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
public class FlatsController : ControllerBase
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public FlatsController(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 50, [FromQuery] string? search = null)
    {
        var tenantId = _currentUser.TenantId;
        if (tenantId == null) return BadRequest("Tenant not selected");

        var query = _context.Flats
            .Include(f => f.Wing)
            .Include(f => f.Members.Where(m => !m.IsDeleted && m.IsActive))
            .Include(f => f.Bills.Where(b => !b.IsDeleted && b.Status != "Cancelled"))
            .Where(f => f.TenantId == tenantId && !f.IsDeleted);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(f => f.FlatNumber.Contains(search));

        var total = await query.CountAsync();
        var flats = await query
            .OrderBy(f => f.FlatNumber)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(f => new FlatDto(
                f.Id, f.FlatNumber, f.Floor, f.CarpetArea, f.BuiltUpArea, f.FlatType,
                f.OccupancyStatus, f.IsActive, f.WingId, f.Wing != null ? f.Wing.Name : null,
                f.Members.Count(m => m.IsActive),
                f.Bills.Where(b => b.Status != "Cancelled").Sum(b => b.GrandTotal - b.AmountPaid)))
            .ToListAsync();

        return Ok(new { items = flats, total, page, pageSize });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var flat = await _context.Flats
            .Include(f => f.Wing)
            .Include(f => f.Members.Where(m => !m.IsDeleted))
            .Include(f => f.Bills.Where(b => !b.IsDeleted).OrderByDescending(b => b.BillDate).Take(10))
            .FirstOrDefaultAsync(f => f.Id == id);

        if (flat == null) return NotFound();

        var outstanding = await _context.Bills
            .Where(b => b.FlatId == id && !b.IsDeleted && b.Status != "Cancelled")
            .SumAsync(b => b.GrandTotal - b.AmountPaid);

        return Ok(new
        {
            flat.Id, flat.FlatNumber, flat.Floor, flat.CarpetArea, flat.BuiltUpArea,
            flat.FlatType, flat.OccupancyStatus, flat.IsActive, flat.WingId,
            WingName = flat.Wing?.Name,
            Members = flat.Members.Select(m => new MemberDto(m.Id, m.FirstName, m.LastName, m.Mobile, m.Email, m.MemberType, m.IsPrimary, m.IsActive, m.FlatId, flat.FlatNumber)),
            BalanceOutstanding = outstanding
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateFlatRequest request)
    {
        var tenantId = _currentUser.TenantId;
        if (tenantId == null) return BadRequest("Tenant not selected");

        var flat = new Flat
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId.Value,
            FlatNumber = request.FlatNumber,
            Floor = request.Floor,
            CarpetArea = request.CarpetArea,
            BuiltUpArea = request.BuiltUpArea,
            FlatType = request.FlatType,
            OccupancyStatus = request.OccupancyStatus,
            WingId = request.WingId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Flats.Add(flat);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = flat.Id }, flat);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateFlatRequest request)
    {
        var flat = await _context.Flats.FindAsync(id);
        if (flat == null) return NotFound();

        flat.FlatNumber = request.FlatNumber;
        flat.Floor = request.Floor;
        flat.CarpetArea = request.CarpetArea;
        flat.BuiltUpArea = request.BuiltUpArea;
        flat.FlatType = request.FlatType;
        flat.OccupancyStatus = request.OccupancyStatus;
        flat.IsActive = request.IsActive;
        flat.WingId = request.WingId;
        flat.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return Ok(flat);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var flat = await _context.Flats.FindAsync(id);
        if (flat == null) return NotFound();

        flat.IsDeleted = true;
        flat.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
