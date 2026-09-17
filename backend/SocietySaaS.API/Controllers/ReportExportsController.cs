using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocietySaaS.Application.Common.Interfaces;
using SocietySaaS.Infrastructure.Services;

namespace SocietySaaS.API.Controllers;

[ApiController]
[Route("api/v1/reports")]
[Authorize]
public class ReportExportsController : ControllerBase
{
    private readonly IReportExcelService _excelService;
    private readonly ICurrentUserService _currentUser;

    public ReportExportsController(IReportExcelService excelService, ICurrentUserService currentUser)
    {
        _excelService = excelService;
        _currentUser = currentUser;
    }

    [HttpGet("bills/excel")]
    public IActionResult GetBillRegister([FromQuery] string? billingPeriod)
    {
        var bytes = _excelService.GenerateBillRegister(billingPeriod, _currentUser.TenantId!.Value);
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "BillRegister.xlsx");
    }

    [HttpGet("payments/excel")]
    public IActionResult GetPaymentRegister()
    {
        var bytes = _excelService.GeneratePaymentRegister(_currentUser.TenantId!.Value);
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "PaymentRegister.xlsx");
    }

    [HttpGet("outstanding/excel")]
    public IActionResult GetOutstandingReport()
    {
        var bytes = _excelService.GenerateOutstandingReport(_currentUser.TenantId!.Value);
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "OutstandingReport.xlsx");
    }

    [HttpGet("members/excel")]
    public IActionResult GetMemberRegister()
    {
        var bytes = _excelService.GenerateMemberRegister(_currentUser.TenantId!.Value);
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "MemberRegister.xlsx");
    }

    [HttpGet("flats/excel")]
    public IActionResult GetFlatRegister()
    {
        var bytes = _excelService.GenerateFlatRegister(_currentUser.TenantId!.Value);
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "FlatRegister.xlsx");
    }
}
