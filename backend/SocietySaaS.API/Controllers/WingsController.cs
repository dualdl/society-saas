using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocietySaaS.Application.Common.DTOs;
using SocietySaaS.Application.Services;
using SocietySaaS.Shared;

namespace SocietySaaS.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class WingsController : ControllerBase
{
    private readonly IWingService _wingService;

    public WingsController(IWingService wingService)
    {
        _wingService = wingService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var wings = await _wingService.GetAllAsync();
            return Ok(ApiResponse<List<WingDto>>.Ok(wings));
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
            var wing = await _wingService.GetByIdAsync(id);
            if (wing == null) return NotFound(ApiResponse<object>.Fail("Wing not found"));
            return Ok(ApiResponse<WingDto>.Ok(wing));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.Fail(ex.Message));
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateWingRequest request)
    {
        try
        {
            var wing = await _wingService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = wing.Id }, ApiResponse<WingDto>.Ok(wing));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.Fail(ex.Message));
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateWingRequest request)
    {
        try
        {
            var wing = await _wingService.UpdateAsync(id, request);
            return Ok(ApiResponse<WingDto>.Ok(wing));
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
            await _wingService.DeleteAsync(id);
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
