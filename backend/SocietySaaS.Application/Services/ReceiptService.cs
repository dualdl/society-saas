using SocietySaaS.Application.Common.DTOs;
using SocietySaaS.Application.Common.Interfaces;
using SocietySaaS.Application.Common.Models;
using SocietySaaS.Domain.Interfaces;

namespace SocietySaaS.Application.Services;

public interface IReceiptService
{
    Task<PaginatedList<ReceiptDto>> GetAllAsync(int page, int pageSize, Guid? flatId);
    Task<ReceiptDto?> GetByIdAsync(Guid id);
}

public class ReceiptService : IReceiptService
{
    private readonly IReceiptRepository _receiptRepository;
    private readonly ICurrentUserService _currentUser;

    public ReceiptService(IReceiptRepository receiptRepository, ICurrentUserService currentUser)
    {
        _receiptRepository = receiptRepository;
        _currentUser = currentUser;
    }

    public async Task<PaginatedList<ReceiptDto>> GetAllAsync(int page, int pageSize, Guid? flatId)
    {
        var receipts = await _receiptRepository.GetAllWithDetailsAsync();
        var tenantId = _currentUser.TenantId;

        var filtered = receipts.Where(r => r.TenantId == tenantId && !r.IsDeleted);
        if (flatId.HasValue)
            filtered = filtered.Where(r => r.FlatId == flatId.Value);

        var total = filtered.Count();
        var paged = filtered
            .OrderByDescending(r => r.ReceiptDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(r => new ReceiptDto(r.Id, r.ReceiptNumber, r.ReceiptDate, r.Amount, r.PaymentMode, r.TransactionReference, r.PdfUrl, r.PaymentId, r.FlatId, r.Flat?.FlatNumber))
            .ToList();

        return new PaginatedList<ReceiptDto>(paged, total, page, pageSize);
    }

    public async Task<ReceiptDto?> GetByIdAsync(Guid id)
    {
        var receipt = await _receiptRepository.GetWithDetailsAsync(id);
        if (receipt == null) return null;
        return new ReceiptDto(receipt.Id, receipt.ReceiptNumber, receipt.ReceiptDate, receipt.Amount, receipt.PaymentMode, receipt.TransactionReference, receipt.PdfUrl, receipt.PaymentId, receipt.FlatId, receipt.Flat?.FlatNumber);
    }
}
