using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocietySaaS.Application.Services;
using SocietySaaS.Domain.Entities;
using SocietySaaS.Shared;

namespace SocietySaaS.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class ImportsController : ControllerBase
{
    private readonly IImportService _service;

    public ImportsController(IImportService service) => _service = service;

    [HttpGet("template")]
    public IActionResult GetTemplate()
    {
        var bytes = _service.GenerateTemplate();
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Society_Onboarding_Template.xlsx");
    }

    [HttpPost("upload")]
    public async Task<ActionResult<ApiResponse<ImportJob>>> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0) return BadRequest(ApiResponse<ImportJob>.Fail("No file uploaded"));
        using var stream = file.OpenReadStream();
        var job = await _service.UploadAndValidateAsync(stream, file.FileName);
        return Ok(ApiResponse<ImportJob>.Ok(job, "File uploaded and validated"));
    }

    [HttpGet("{jobId}")]
    public async Task<ActionResult<ApiResponse<ImportJob?>>> GetJob(Guid jobId)
    {
        var job = await _service.GetJobAsync(jobId);
        return Ok(ApiResponse<ImportJob?>.Ok(job));
    }

    [HttpPost("{jobId}/confirm")]
    public async Task<ActionResult<ApiResponse<ImportJob>>> Confirm(Guid jobId)
    {
        var job = await _service.ConfirmImportAsync(jobId);
        return Ok(ApiResponse<ImportJob>.Ok(job, "Import completed"));
    }
}
