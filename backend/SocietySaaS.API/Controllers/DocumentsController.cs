using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocietySaaS.Application.Common.Interfaces;
using SocietySaaS.Domain.Entities;
using SocietySaaS.Domain.Interfaces;
using SocietySaaS.Infrastructure.Storage;
using SocietySaaS.Shared;

namespace SocietySaaS.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class DocumentsController : ControllerBase
{
    private readonly IBlobStorageService _blobStorage;
    private readonly IDocumentRepository _documentRepository;
    private readonly ICurrentUserService _currentUser;

    public DocumentsController(IBlobStorageService blobStorage, IDocumentRepository documentRepository, ICurrentUserService currentUser)
    {
        _blobStorage = blobStorage;
        _documentRepository = documentRepository;
        _currentUser = currentUser;
    }

    [HttpPost("upload")]
    public async Task<ActionResult<ApiResponse<Document>>> Upload(IFormFile file, [FromQuery] string category = "General")
    {
        if (file == null || file.Length == 0) return BadRequest(ApiResponse<Document>.Fail("No file uploaded"));

        var tenantId = _currentUser.TenantId!.Value;
        var blobName = $"{tenantId}/{category}/{Guid.NewGuid()}_{file.FileName}";
        using var stream = file.OpenReadStream();
        var url = await _blobStorage.UploadAsync("documents", blobName, stream, file.ContentType);

        var doc = new Document
        {
            Name = file.FileName, Category = category, BlobUrl = url,
            ContentType = file.ContentType, FileSize = file.Length,
            TenantId = tenantId
        };
        await _documentRepository.AddAsync(doc);

        return Ok(ApiResponse<Document>.Ok(doc, "File uploaded"));
    }

    [HttpGet("{id}/download")]
    public async Task<IActionResult> Download(Guid id)
    {
        var doc = await _documentRepository.GetByIdAsync(id);
        if (doc == null) return NotFound();

        var blobName = new Uri(doc.BlobUrl!).AbsolutePath.TrimStart('/');
        var stream = await _blobStorage.DownloadAsync("documents", blobName);
        if (stream == null) return NotFound();

        return File(stream, doc.ContentType, doc.Name);
    }
}
