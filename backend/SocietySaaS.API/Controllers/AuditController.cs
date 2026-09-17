using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocietySaaS.Application.Services;
using SocietySaaS.Domain.Entities;
using SocietySaaS.Shared;

namespace SocietySaaS.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class AuditController : ControllerBase
{
    private readonly IAuditService _auditService;

    public AuditController(IAuditService auditService)
    {
        _auditService = auditService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<AuditLog>>>> GetLogs(
        [FromQuery] string? entityType, [FromQuery] Guid? entityId,
        [FromQuery] string? userId, [FromQuery] int limit = 100)
    {
        var logs = await _auditService.GetLogsAsync(entityType, entityId, userId, limit);
        return Ok(ApiResponse<List<AuditLog>>.Ok(logs));
    }
}
