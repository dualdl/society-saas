using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocietySaaS.Application.Common.DTOs;
using SocietySaaS.Application.Services;
using SocietySaaS.Shared;

namespace SocietySaaS.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class ReceiptsController : ControllerBase
{
    private readonly IReceiptService _receiptService;

    public ReceiptsController(IReceiptService receiptService)
    {
        _receiptService = receiptService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] Guid? flatId = null)
    {
        try
        {
            var result = await _receiptService.GetAllAsync(page, pageSize, flatId);
            return Ok(ApiResponse<object>.Ok(new { items = result.Items, total = result.TotalCount, page = result.PageNumber, pageSize = result.PageSize }));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.Fail(ex.Message));
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var receipt = await _receiptService.GetByIdAsync(id);
            if (receipt == null) return NotFound(ApiResponse<object>.Fail("Receipt not found"));
            return Ok(ApiResponse<ReceiptDto>.Ok(receipt));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.Fail(ex.Message));
        }
    }
}
