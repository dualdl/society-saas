using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocietySaaS.Application.Services;
using SocietySaaS.Domain.Entities;
using SocietySaaS.Shared;

namespace SocietySaaS.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class LatePaymentRulesController : ControllerBase
{
    private readonly ILatePaymentRuleService _service;

    public LatePaymentRulesController(ILatePaymentRuleService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<LatePaymentRule?>>> Get()
    {
        var rule = await _service.GetRuleAsync();
        return Ok(ApiResponse<LatePaymentRule?>.Ok(rule));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<LatePaymentRule>>> CreateOrUpdate([FromBody] LatePaymentRuleRequest request)
    {
        var rule = await _service.CreateOrUpdateAsync(request.Percentage, request.GracePeriodDays, request.CalculationType, request.MaximumFine, request.IsEnabled);
        return Ok(ApiResponse<LatePaymentRule>.Ok(rule, "Late payment rule saved"));
    }
}

public record LatePaymentRuleRequest
{
    public decimal Percentage { get; init; }
    public int GracePeriodDays { get; init; }
    public string CalculationType { get; init; } = "OutstandingPrincipal";
    public decimal? MaximumFine { get; init; }
    public bool IsEnabled { get; init; }
}
