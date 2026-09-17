using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocietySaaS.Application.Common.DTOs;
using SocietySaaS.Application.Services;
using SocietySaaS.Shared;

namespace SocietySaaS.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class FlatsController : ControllerBase
{
    private readonly IFlatService _flatService;

    public FlatsController(IFlatService flatService)
    {
        _flatService = flatService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 50, [FromQuery] string? search = null)
    {
        try
        {
            var result = await _flatService.GetAllAsync(page, pageSize, search);
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
            var flat = await _flatService.GetByIdAsync(id);
            if (flat == null) return NotFound(ApiResponse<object>.Fail("Flat not found"));
            return Ok(ApiResponse<FlatDto>.Ok(flat));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.Fail(ex.Message));
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateFlatRequest request)
    {
        try
        {
            var flat = await _flatService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = flat.Id }, ApiResponse<FlatDto>.Ok(flat));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.Fail(ex.Message));
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateFlatRequest request)
    {
        try
        {
            var flat = await _flatService.UpdateAsync(id, request);
            return Ok(ApiResponse<FlatDto>.Ok(flat));
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

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _flatService.DeleteAsync(id);
            return NoContent();
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
