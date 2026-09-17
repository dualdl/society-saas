using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocietySaaS.Application.Common.DTOs;
using SocietySaaS.Application.Common.Interfaces;
using SocietySaaS.Application.Services;
using SocietySaaS.Shared;

namespace SocietySaaS.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class SettingsController : ControllerBase
{
    private readonly ITenantService _tenantService;
    private readonly ICurrentUserService _currentUser;

    public SettingsController(ITenantService tenantService, ICurrentUserService currentUser)
    {
        _tenantService = tenantService;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<IActionResult> GetSettings()
    {
        try
        {
            if (!_currentUser.TenantId.HasValue)
                return BadRequest(ApiResponse<object>.Fail("No society context"));

            var settings = await _tenantService.GetSettingsAsync(_currentUser.TenantId.Value);
            if (settings == null) return NotFound(ApiResponse<object>.Fail("Society not found"));
            return Ok(ApiResponse<SocietySettingsDto>.Ok(settings));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.Fail(ex.Message));
        }
    }

    [HttpPut]
    public async Task<IActionResult> UpdateSettings([FromBody] UpdateSocietySettingsRequest request)
    {
        try
        {
            if (!_currentUser.TenantId.HasValue)
                return BadRequest(ApiResponse<object>.Fail("No society context"));

            var settings = await _tenantService.UpdateSettingsAsync(_currentUser.TenantId.Value, request);
            return Ok(ApiResponse<SocietySettingsDto>.Ok(settings));
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
