using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocietySaaS.Application.Common.Interfaces;
using SocietySaaS.Domain.Entities;
using SocietySaaS.Domain.Interfaces;
using SocietySaaS.Shared;

namespace SocietySaaS.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class EmailsController : ControllerBase
{
    private readonly IEmailQueueRepository _emailQueueRepository;
    private readonly IEmailTemplateRepository _emailTemplateRepository;

    public EmailsController(IEmailQueueRepository emailQueueRepository, IEmailTemplateRepository emailTemplateRepository)
    {
        _emailQueueRepository = emailQueueRepository;
        _emailTemplateRepository = emailTemplateRepository;
    }

    [HttpGet("queue")]
    public async Task<ActionResult<ApiResponse<List<EmailQueue>>>> GetQueue()
    {
        var emails = await _emailQueueRepository.GetAllAsync();
        return Ok(ApiResponse<List<EmailQueue>>.Ok(emails.ToList()));
    }

    [HttpGet("templates")]
    public async Task<ActionResult<ApiResponse<List<EmailTemplate>>>> GetTemplates()
    {
        var templates = await _emailTemplateRepository.GetAllAsync();
        return Ok(ApiResponse<List<EmailTemplate>>.Ok(templates.ToList()));
    }

    [HttpPost("templates")]
    public async Task<ActionResult<ApiResponse<EmailTemplate>>> CreateTemplate([FromBody] EmailTemplate template)
    {
        await _emailTemplateRepository.AddAsync(template);
        return Ok(ApiResponse<EmailTemplate>.Ok(template, "Template created"));
    }
}
