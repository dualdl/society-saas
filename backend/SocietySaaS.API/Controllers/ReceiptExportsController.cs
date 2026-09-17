using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocietySaaS.Application.Common.Interfaces;
using SocietySaaS.Domain.Interfaces;
using SocietySaaS.Infrastructure.Services;

namespace SocietySaaS.API.Controllers;

[ApiController]
[Route("api/v1/receipts")]
[Authorize]
public class ReceiptExportsController : ControllerBase
{
    private readonly IReceiptPdfService _pdfService;
    private readonly IReceiptRepository _receiptRepository;
    private readonly ITenantRepository _tenantRepository;
    private readonly ICurrentUserService _currentUser;

    public ReceiptExportsController(IReceiptPdfService pdfService, IReceiptRepository receiptRepository, ITenantRepository tenantRepository, ICurrentUserService currentUser)
    {
        _pdfService = pdfService;
        _receiptRepository = receiptRepository;
        _tenantRepository = tenantRepository;
        _currentUser = currentUser;
    }

    [HttpGet("{id}/pdf")]
    public async Task<IActionResult> GetPdf(Guid id)
    {
        var receipt = await _receiptRepository.GetWithDetailsAsync(id);
        if (receipt == null) return NotFound();

        var tenant = await _tenantRepository.GetByIdAsync(_currentUser.TenantId!.Value);
        var bytes = _pdfService.GenerateReceiptPdf(receipt, tenant?.Name ?? "Society", tenant?.Address ?? "");
        return File(bytes, "application/pdf", $"Receipt_{receipt.ReceiptNumber}.pdf");
    }
}
