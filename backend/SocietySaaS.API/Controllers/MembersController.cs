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
public class MembersController : ControllerBase
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public MembersController(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 50, [FromQuery] string? search = null, [FromQuery] Guid? flatId = null)
    {
        var tenantId = _currentUser.TenantId;
        if (tenantId == null) return BadRequest("Tenant not selected");

        var query = _context.Members
            .Include(m => m.Flat)
            .Where(m => m.TenantId == tenantId && !m.IsDeleted);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(m => m.FirstName.Contains(search) || (m.LastName != null && m.LastName.Contains(search)) || m.Mobile.Contains(search));

        if (flatId.HasValue)
            query = query.Where(m => m.FlatId == flatId.Value);

        var total = await query.CountAsync();
        var members = await query
            .OrderBy(m => m.FirstName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(m => new MemberDto(m.Id, m.FirstName, m.LastName, m.Mobile, m.Email, m.MemberType, m.IsPrimary, m.IsActive, m.FlatId, m.Flat.FlatNumber))
            .ToListAsync();

        return Ok(new { items = members, total, page, pageSize });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var member = await _context.Members
            .Include(m => m.Flat)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (member == null) return NotFound();

        return Ok(new MemberDto(member.Id, member.FirstName, member.LastName, member.Mobile, member.Email, member.MemberType, member.IsPrimary, member.IsActive, member.FlatId, member.Flat?.FlatNumber));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMemberRequest request)
    {
        var tenantId = _currentUser.TenantId;
        if (tenantId == null) return BadRequest("Tenant not selected");

        var member = new Member
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId.Value,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Mobile = request.Mobile,
            Email = request.Email,
            MemberType = request.MemberType,
            IsPrimary = request.IsPrimary,
            FlatId = request.FlatId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Members.Add(member);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = member.Id }, member);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMemberRequest request)
    {
        var member = await _context.Members.FindAsync(id);
        if (member == null) return NotFound();

        member.FirstName = request.FirstName;
        member.LastName = request.LastName;
        member.Mobile = request.Mobile;
        member.Email = request.Email;
        member.MemberType = request.MemberType;
        member.IsPrimary = request.IsPrimary;
        member.IsActive = request.IsActive;
        member.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return Ok(member);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var member = await _context.Members.FindAsync(id);
        if (member == null) return NotFound();

        member.IsDeleted = true;
        member.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
