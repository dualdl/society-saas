using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocietySaaS.Application.Common.DTOs;
using SocietySaaS.Application.Services;
using SocietySaaS.Shared;

namespace SocietySaaS.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class AccountingController : ControllerBase
{
    private readonly IAccountingService _service;

    public AccountingController(IAccountingService service) => _service = service;

    [HttpGet("ledger")]
    public async Task<ActionResult<ApiResponse<List<LedgerEntryDto>>>> GetGeneralLedger([FromQuery] Guid? flatId)
    {
        var result = await _service.GetGeneralLedgerAsync(flatId);
        return Ok(ApiResponse<List<LedgerEntryDto>>.Ok(result));
    }

    [HttpGet("trial-balance")]
    public async Task<ActionResult<ApiResponse<TrialBalanceDto>>> GetTrialBalance()
    {
        var result = await _service.GetTrialBalanceAsync();
        return Ok(ApiResponse<TrialBalanceDto>.Ok(result));
    }

    [HttpGet("member-ledger/{flatId}")]
    public async Task<ActionResult<ApiResponse<List<LedgerEntryDto>>>> GetMemberLedger(Guid flatId)
    {
        var result = await _service.GetMemberLedgerAsync(flatId);
        return Ok(ApiResponse<List<LedgerEntryDto>>.Ok(result));
    }
}
