using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocietySaaS.Application.Common.DTOs;
using SocietySaaS.Application.Services;
using SocietySaaS.Shared;

namespace SocietySaaS.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class BillsController : ControllerBase
{
    private readonly IBillService _billService;

    public BillsController(IBillService billService)
    {
        _billService = billService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? status = null, [FromQuery] string? billingPeriod = null, [FromQuery] Guid? flatId = null)
    {
        try
        {
            var result = await _billService.GetAllAsync(page, pageSize, status, billingPeriod, flatId);
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
            var bill = await _billService.GetByIdAsync(id);
            if (bill == null) return NotFound(ApiResponse<object>.Fail("Bill not found"));
            return Ok(ApiResponse<BillDto>.Ok(bill));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.Fail(ex.Message));
        }
    }

    [HttpPost("generate")]
    public async Task<IActionResult> GenerateBill([FromBody] GenerateBillRequest request)
    {
        try
        {
            var bill = await _billService.GenerateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = bill.Id }, ApiResponse<BillDto>.Ok(bill));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<object>.Fail(ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.Fail(ex.Message));
        }
    }

    [HttpPost("bulk-generate")]
    public async Task<IActionResult> BulkGenerate([FromBody] BulkGenerateBillsRequest request)
    {
        try
        {
            var count = await _billService.BulkGenerateAsync(request);
            return Ok(ApiResponse<object>.Ok(new { message = $"{count} bills generated", billCount = count }));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.Fail(ex.Message));
        }
    }

    [HttpPut("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] string status)
    {
        try
        {
            var bill = await _billService.UpdateStatusAsync(id, status);
            return Ok(ApiResponse<BillDto>.Ok(bill));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<object>.Fail(ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.Fail(ex.Message));
        }
    }

    [HttpPut("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id, [FromBody] string reason)
    {
        try
        {
            var bill = await _billService.CancelAsync(id, reason);
            return Ok(ApiResponse<BillDto>.Ok(bill));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<object>.Fail(ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.Fail(ex.Message));
        }
    }
}
