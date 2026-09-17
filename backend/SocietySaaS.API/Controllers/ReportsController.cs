using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocietySaaS.Application.Common.DTOs;
using SocietySaaS.Application.Services;
using SocietySaaS.Shared;

namespace SocietySaaS.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet("revenue")]
    public async Task<IActionResult> GetRevenueReport([FromQuery] string? billingPeriod = null)
    {
        try
        {
            var report = await _reportService.GetRevenueReportAsync(billingPeriod);
            if (report == null) return BadRequest(ApiResponse<object>.Fail("Tenant not selected"));
            return Ok(ApiResponse<RevenueReportDto>.Ok(report));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.Fail(ex.Message));
        }
    }

    [HttpGet("outstanding")]
    public async Task<IActionResult> GetOutstandingReport()
    {
        try
        {
            var report = await _reportService.GetOutstandingReportAsync();
            if (report == null) return BadRequest(ApiResponse<object>.Fail("Tenant not selected"));
            return Ok(ApiResponse<OutstandingReportDto>.Ok(report));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.Fail(ex.Message));
        }
    }
}
