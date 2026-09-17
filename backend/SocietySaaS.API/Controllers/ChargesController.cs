using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocietySaaS.Application.Common.DTOs;
using SocietySaaS.Application.Services;
using SocietySaaS.Shared;

namespace SocietySaaS.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class ChargesController : ControllerBase
{
    private readonly IChargeService _chargeService;

    public ChargesController(IChargeService chargeService)
    {
        _chargeService = chargeService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var charges = await _chargeService.GetAllAsync();
            return Ok(ApiResponse<List<ChargeDto>>.Ok(charges));
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
            var charge = await _chargeService.GetByIdAsync(id);
            if (charge == null) return NotFound(ApiResponse<object>.Fail("Charge not found"));
            return Ok(ApiResponse<ChargeDto>.Ok(charge));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.Fail(ex.Message));
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateChargeRequest request)
    {
        try
        {
            var charge = await _chargeService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = charge.Id }, ApiResponse<ChargeDto>.Ok(charge));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.Fail(ex.Message));
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateChargeRequest request)
    {
        try
        {
            var charge = await _chargeService.UpdateAsync(id, request);
            return Ok(ApiResponse<ChargeDto>.Ok(charge));
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
            await _chargeService.DeleteAsync(id);
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
