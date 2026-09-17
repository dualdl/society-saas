using ClosedXML.Excel;
using SocietySaaS.Application.Common.Interfaces;

namespace SocietySaaS.Infrastructure.Services;

public interface IReportExcelService
{
    byte[] GenerateBillRegister(string? billingPeriod, Guid tenantId);
    byte[] GeneratePaymentRegister(Guid tenantId);
    byte[] GenerateOutstandingReport(Guid tenantId);
    byte[] GenerateMemberRegister(Guid tenantId);
    byte[] GenerateFlatRegister(Guid tenantId);
}

public class ReportExcelService : IReportExcelService
{
    private readonly IApplicationDbContext _context;

    public ReportExcelService(IApplicationDbContext context) => _context = context;

    public byte[] GenerateBillRegister(string? billingPeriod, Guid tenantId)
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Bill Register");
        ws.Cell(1, 1).Value = "Bill Number"; ws.Cell(1, 2).Value = "Billing Period";
        ws.Cell(1, 3).Value = "Flat"; ws.Cell(1, 4).Value = "Bill Date";
        ws.Cell(1, 5).Value = "Due Date"; ws.Cell(1, 6).Value = "Grand Total";
        ws.Cell(1, 7).Value = "Amount Paid"; ws.Cell(1, 8).Value = "Balance"; ws.Cell(1, 9).Value = "Status";
        ws.Row(1).Style.Font.Bold = true;

        var bills = _context.Bills.Where(b => b.TenantId == tenantId && !b.IsDeleted).ToList();
        if (!string.IsNullOrEmpty(billingPeriod)) bills = bills.Where(b => b.BillingPeriod == billingPeriod).ToList();

        int row = 2;
        foreach (var bill in bills)
        {
            ws.Cell(row, 1).Value = bill.BillNumber; ws.Cell(row, 2).Value = bill.BillingPeriod;
            ws.Cell(row, 3).Value = bill.FlatId.ToString(); ws.Cell(row, 4).Value = bill.BillDate;
            ws.Cell(row, 5).Value = bill.DueDate; ws.Cell(row, 6).Value = (double)bill.GrandTotal;
            ws.Cell(row, 7).Value = (double)bill.AmountPaid; ws.Cell(row, 8).Value = (double)bill.BalanceOutstanding;
            ws.Cell(row, 9).Value = bill.Status;
            row++;
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public byte[] GeneratePaymentRegister(Guid tenantId)
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Payment Register");
        ws.Cell(1, 1).Value = "Payment Number"; ws.Cell(1, 2).Value = "Date";
        ws.Cell(1, 3).Value = "Flat"; ws.Cell(1, 4).Value = "Amount";
        ws.Cell(1, 5).Value = "Mode"; ws.Cell(1, 6).Value = "Status";
        ws.Row(1).Style.Font.Bold = true;

        var payments = _context.Payments.Where(p => p.TenantId == tenantId && !p.IsDeleted).ToList();
        int row = 2;
        foreach (var p in payments)
        {
            ws.Cell(row, 1).Value = p.PaymentNumber; ws.Cell(row, 2).Value = p.PaymentDate;
            ws.Cell(row, 3).Value = p.FlatId.ToString(); ws.Cell(row, 4).Value = (double)p.Amount;
            ws.Cell(row, 5).Value = p.PaymentMode; ws.Cell(row, 6).Value = p.Status;
            row++;
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public byte[] GenerateOutstandingReport(Guid tenantId)
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Outstanding");
        ws.Cell(1, 1).Value = "Flat"; ws.Cell(1, 2).Value = "Outstanding";
        ws.Cell(1, 3).Value = "Days Overdue";
        ws.Row(1).Style.Font.Bold = true;

        var bills = _context.Bills.Where(b => b.TenantId == tenantId && !b.IsDeleted && b.BalanceOutstanding > 0).ToList();
        int row = 2;
        foreach (var bill in bills)
        {
            ws.Cell(row, 1).Value = bill.FlatId.ToString(); ws.Cell(row, 2).Value = (double)bill.BalanceOutstanding;
            ws.Cell(row, 3).Value = bill.DueDate < DateTime.UtcNow ? (DateTime.UtcNow - bill.DueDate).Days : 0;
            row++;
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public byte[] GenerateMemberRegister(Guid tenantId)
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Members");
        ws.Cell(1, 1).Value = "Name"; ws.Cell(1, 2).Value = "Mobile";
        ws.Cell(1, 3).Value = "Email"; ws.Cell(1, 4).Value = "Type";
        ws.Cell(1, 5).Value = "Flat"; ws.Row(1).Style.Font.Bold = true;

        var members = _context.Members.Where(m => m.TenantId == tenantId && !m.IsDeleted).ToList();
        int row = 2;
        foreach (var m in members)
        {
            ws.Cell(row, 1).Value = $"{m.FirstName} {m.LastName}"; ws.Cell(row, 2).Value = m.Mobile;
            ws.Cell(row, 3).Value = m.Email; ws.Cell(row, 4).Value = m.MemberType;
            ws.Cell(row, 5).Value = m.FlatId.ToString(); row++;
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public byte[] GenerateFlatRegister(Guid tenantId)
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Flats");
        ws.Cell(1, 1).Value = "Flat Number"; ws.Cell(1, 2).Value = "Floor";
        ws.Cell(1, 3).Value = "Carpet Area"; ws.Cell(1, 4).Value = "Status";
        ws.Row(1).Style.Font.Bold = true;

        var flats = _context.Flats.Where(f => f.TenantId == tenantId && !f.IsDeleted).ToList();
        int row = 2;
        foreach (var f in flats)
        {
            ws.Cell(row, 1).Value = f.FlatNumber; ws.Cell(row, 2).Value = f.Floor;
            ws.Cell(row, 3).Value = (double)f.CarpetArea; ws.Cell(row, 4).Value = f.OccupancyStatus;
            row++;
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
