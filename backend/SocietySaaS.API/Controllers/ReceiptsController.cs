using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocietySaaS.Application.Common.DTOs;
using SocietySaaS.Application.Common.Interfaces;

namespace SocietySaaS.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class ReceiptsController : ControllerBase
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public ReceiptsController(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] Guid? flatId = null)
    {
        var tenantId = _currentUser.TenantId;
        if (tenantId == null) return BadRequest("Tenant not selected");

        var query = _context.Receipts
            .Include(r => r.Flat)
            .Where(r => r.TenantId == tenantId && !r.IsDeleted);

        if (flatId.HasValue)
            query = query.Where(r => r.FlatId == flatId.Value);

        var total = await query.CountAsync();
        var receipts = await query
            .OrderByDescending(r => r.ReceiptDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(r => new ReceiptDto(r.Id, r.ReceiptNumber, r.ReceiptDate, r.Amount, r.PaymentMode, r.TransactionReference, r.PdfUrl, r.PaymentId, r.FlatId, r.Flat.FlatNumber))
            .ToListAsync();

        return Ok(new { items = receipts, total, page, pageSize });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var receipt = await _context.Receipts
            .Include(r => r.Flat)
            .Include(r => r.Payment)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (receipt == null) return NotFound();

        return Ok(new ReceiptDto(receipt.Id, receipt.ReceiptNumber, receipt.ReceiptDate, receipt.Amount, receipt.PaymentMode, receipt.TransactionReference, receipt.PdfUrl, receipt.PaymentId, receipt.FlatId, receipt.Flat?.FlatNumber));
    }
}
