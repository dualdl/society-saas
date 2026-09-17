using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using SocietySaaS.Application.Common.Interfaces;
using SocietySaaS.Domain.Entities;
using SocietySaaS.Domain.Interfaces;

namespace SocietySaaS.Application.Services;

public interface IImportService
{
    Task<ImportJob> UploadAndValidateAsync(Stream fileStream, string fileName);
    Task<ImportJob?> GetJobAsync(Guid jobId);
    Task<ImportJob> ConfirmImportAsync(Guid jobId);
    byte[] GenerateTemplate();
}

public class ImportService : IImportService
{
    private readonly IApplicationDbContext _context;
    private readonly IImportJobRepository _importJobRepository;
    private readonly ICurrentUserService _currentUser;

    public ImportService(IApplicationDbContext context, IImportJobRepository importJobRepository, ICurrentUserService currentUser)
    {
        _context = context;
        _importJobRepository = importJobRepository;
        _currentUser = currentUser;
    }

    public byte[] GenerateTemplate()
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("FlatMemberData");
        ws.Cell(1, 1).Value = "Wing"; ws.Cell(1, 2).Value = "FlatNumber"; ws.Cell(1, 3).Value = "Floor";
        ws.Cell(1, 4).Value = "CarpetArea"; ws.Cell(1, 5).Value = "BuiltUpArea";
        ws.Cell(1, 6).Value = "OwnerName"; ws.Cell(1, 7).Value = "Mobile"; ws.Cell(1, 8).Value = "Email";
        ws.Cell(1, 9).Value = "OccupancyStatus"; ws.Cell(1, 10).Value = "LastYearOutstanding";
        ws.Cell(1, 11).Value = "OutstandingType"; ws.Cell(1, 12).Value = "OutstandingAsOfDate";
        ws.Row(1).Style.Font.Bold = true;

        var wsSample = workbook.Worksheets.Add("SampleData");
        wsSample.Cell(1, 1).Value = "Wing"; wsSample.Cell(1, 2).Value = "FlatNumber"; wsSample.Cell(1, 3).Value = "Floor";
        wsSample.Cell(1, 4).Value = "CarpetArea"; wsSample.Cell(1, 5).Value = "BuiltUpArea";
        wsSample.Cell(1, 6).Value = "OwnerName"; wsSample.Cell(1, 7).Value = "Mobile"; wsSample.Cell(1, 8).Value = "Email";
        wsSample.Cell(1, 9).Value = "OccupancyStatus"; wsSample.Cell(1, 10).Value = "LastYearOutstanding";
        wsSample.Cell(1, 11).Value = "OutstandingType"; wsSample.Cell(1, 12).Value = "OutstandingAsOfDate";
        wsSample.Row(1).Style.Font.Bold = true;
        wsSample.Cell(2, 1).Value = "A"; wsSample.Cell(2, 2).Value = "101"; wsSample.Cell(2, 3).Value = 1;
        wsSample.Cell(2, 4).Value = 650; wsSample.Cell(2, 5).Value = 800;
        wsSample.Cell(2, 6).Value = "Rajesh Patil"; wsSample.Cell(2, 7).Value = "9876543210";
        wsSample.Cell(2, 8).Value = "rajesh@email.com"; wsSample.Cell(2, 9).Value = "Owner";
        wsSample.Cell(2, 10).Value = 12500; wsSample.Cell(2, 11).Value = "Debit";
        wsSample.Cell(2, 12).Value = new DateTime(2026, 3, 31);

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public async Task<ImportJob> UploadAndValidateAsync(Stream fileStream, string fileName)
    {
        var tenantId = _currentUser.TenantId!.Value;
        var job = new ImportJob
        {
            FileName = fileName, JobType = "Onboarding", Status = "Processing",
            BlobUrl = "", TenantId = tenantId
        };
        await _importJobRepository.AddAsync(job);

        using var workbook = new XLWorkbook(fileStream);
        var worksheet = workbook.Worksheet("FlatMemberData");
        if (worksheet == null) throw new InvalidOperationException("Sheet 'FlatMemberData' not found");

        var rows = worksheet.RangeUsed().RowsUsed().Skip(1).ToList();
        job.TotalRows = rows.Count;

        var importRows = new List<ImportRow>();
        foreach (var row in rows)
        {
            var rowData = string.Join("|",
                row.Cell(1).GetString(), row.Cell(2).GetString(), row.Cell(3).GetString(),
                row.Cell(4).GetString(), row.Cell(5).GetString(), row.Cell(6).GetString(),
                row.Cell(7).GetString(), row.Cell(8).GetString(), row.Cell(9).GetString(),
                row.Cell(10).GetString(), row.Cell(11).GetString(), row.Cell(12).GetString());

            var errors = ValidateRow(row);
            importRows.Add(new ImportRow
            {
                RowNumber = row.RowNumber(), RawData = rowData,
                Status = errors.Any() ? "Error" : "Valid",
                ErrorMessage = errors.Any() ? string.Join("; ", errors) : null,
                ImportJobId = job.Id
            });
        }

        foreach (var row in importRows)
            await _context.ImportRows.AddAsync(row);

        job.ErrorRows = importRows.Count(r => r.Status == "Error");
        job.ProcessedRows = importRows.Count(r => r.Status == "Valid");
        job.Status = "Pending";
        await _context.SaveChangesAsync();

        return await _importJobRepository.GetWithRowsAsync(job.Id) ?? job;
    }

    public async Task<ImportJob?> GetJobAsync(Guid jobId)
    {
        return await _importJobRepository.GetWithRowsAsync(jobId);
    }

    public async Task<ImportJob> ConfirmImportAsync(Guid jobId)
    {
        var job = await _importJobRepository.GetWithRowsAsync(jobId);
        if (job == null) throw new KeyNotFoundException("Import job not found");

        job.Status = "Processing";
        await _context.SaveChangesAsync();

        var validRows = job.ImportRows.Where(r => r.Status == "Valid").ToList();
        var tenantId = _currentUser.TenantId!.Value;

        foreach (var row in validRows)
        {
            var parts = row.RawData!.Split('|');
            var wingName = parts[0];
            var flatNumber = parts[1];
            var floor = int.TryParse(parts[2], out var f) ? f : 1;
            var carpetArea = decimal.TryParse(parts[3], out var ca) ? ca : 0;
            var builtUpArea = decimal.TryParse(parts[4], out var ba) ? ba : 0;
            var ownerName = parts[5];
            var mobile = parts[6];
            var email = parts[7];

            var wing = await _context.Wings.FirstOrDefaultAsync(w => w.Name == wingName && w.TenantId == tenantId);
            if (wing == null)
            {
                wing = new Wing { Name = wingName, TotalFloors = 10, FlatsPerFloor = 10, TenantId = tenantId };
                await _context.Wings.AddAsync(wing);
                await _context.SaveChangesAsync();
            }

            var flat = new Flat
            {
                FlatNumber = flatNumber, Floor = floor, CarpetArea = carpetArea, BuiltUpArea = builtUpArea,
                OccupancyStatus = parts[8], WingId = wing.Id, TenantId = tenantId
            };
            await _context.Flats.AddAsync(flat);
            await _context.SaveChangesAsync();

            var member = new Member
            {
                FirstName = ownerName, Mobile = mobile, Email = email,
                MemberType = "Owner", IsPrimary = true, FlatId = flat.Id, TenantId = tenantId
            };
            await _context.Members.AddAsync(member);

            if (decimal.TryParse(parts[9], out var outstanding) && outstanding > 0)
            {
                var ob = new OpeningBalance
                {
                    Amount = outstanding, BalanceType = parts[10] == "Credit" ? "Credit" : "Debit",
                    AsOfDate = DateTime.TryParse(parts[11], out var dt) ? dt : DateTime.UtcNow,
                    FlatId = flat.Id, TenantId = tenantId
                };
                await _context.OpeningBalances.AddAsync(ob);
            }
        }

        job.Status = "Completed";
        job.CompletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return job;
    }

    private static List<string> ValidateRow(IXLRangeRow row)
    {
        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(row.Cell(1).GetString())) errors.Add("Wing is required");
        if (string.IsNullOrWhiteSpace(row.Cell(2).GetString())) errors.Add("FlatNumber is required");
        if (string.IsNullOrWhiteSpace(row.Cell(6).GetString())) errors.Add("OwnerName is required");
        return errors;
    }
}
