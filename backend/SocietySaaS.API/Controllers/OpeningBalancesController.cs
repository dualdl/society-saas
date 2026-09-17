using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocietySaaS.Application.Common.DTOs;
using SocietySaaS.Application.Services;
using SocietySaaS.Shared;

namespace SocietySaaS.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class OpeningBalancesController : ControllerBase
{
    private readonly IOpeningBalanceService _service;

    public OpeningBalancesController(IOpeningBalanceService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<OpeningBalanceDto>>>> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(ApiResponse<List<OpeningBalanceDto>>.Ok(result));
    }

    [HttpGet("flat/{flatId}")]
    public async Task<ActionResult<ApiResponse<OpeningBalanceDto?>>> GetByFlat(Guid flatId)
    {
        var result = await _service.GetByFlatAsync(flatId);
        return Ok(ApiResponse<OpeningBalanceDto?>.Ok(result));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<OpeningBalanceDto>>> Set([FromBody] SetOpeningBalanceRequest request)
    {
        var result = await _service.SetAsync(request);
        return Ok(ApiResponse<OpeningBalanceDto>.Ok(result, "Opening balance saved"));
    }
}
