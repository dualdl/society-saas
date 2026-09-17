using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocietySaaS.Application.Common.DTOs;
using SocietySaaS.Application.Services;
using SocietySaaS.Shared;

namespace SocietySaaS.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class TenantsController : ControllerBase
{
    private readonly ITenantService _tenantService;

    public TenantsController(ITenantService tenantService)
    {
        _tenantService = tenantService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll([FromQuery] string? slug = null)
    {
        try
        {
            if (!string.IsNullOrEmpty(slug))
            {
                var tenants = await _tenantService.GetAllAsync();
                var tenant = tenants.FirstOrDefault(t => t.Slug == slug);
                if (tenant == null) return NotFound(ApiResponse<object>.Fail("Society not found"));
                return Ok(ApiResponse<TenantDto>.Ok(tenant));
            }

            var allTenants = await _tenantService.GetAllAsync();
            return Ok(ApiResponse<List<TenantDto>>.Ok(allTenants));
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
            var tenant = await _tenantService.GetByIdAsync(id);
            if (tenant == null) return NotFound(ApiResponse<object>.Fail("Tenant not found"));
            return Ok(ApiResponse<TenantDto>.Ok(tenant));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.Fail(ex.Message));
        }
    }

    [HttpPost]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> Create([FromBody] CreateTenantRequest request)
    {
        try
        {
            var result = await _tenantService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), result, ApiResponse<object>.Ok(result));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.Fail(ex.Message));
        }
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTenantRequest request)
    {
        try
        {
            var tenant = await _tenantService.UpdateAsync(id, request);
            return Ok(ApiResponse<TenantDto>.Ok(tenant));
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
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _tenantService.DeleteAsync(id);
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
