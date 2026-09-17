using SocietySaaS.Application.Common.DTOs;
using SocietySaaS.Application.Common.Interfaces;
using SocietySaaS.Domain.Entities;
using SocietySaaS.Domain.Interfaces;

namespace SocietySaaS.Application.Services;

public interface IAccountingService
{
    Task<List<LedgerEntryDto>> GetGeneralLedgerAsync(Guid? flatId);
    Task<TrialBalanceDto> GetTrialBalanceAsync();
    Task<List<LedgerEntryDto>> GetMemberLedgerAsync(Guid flatId);
}

public class AccountingService : IAccountingService
{
    private readonly ILedgerEntryRepository _ledgerRepository;
    private readonly ICurrentUserService _currentUser;

    public AccountingService(ILedgerEntryRepository ledgerRepository, ICurrentUserService currentUser)
    {
        _ledgerRepository = ledgerRepository;
        _currentUser = currentUser;
    }

    public async Task<List<LedgerEntryDto>> GetGeneralLedgerAsync(Guid? flatId)
    {
        var tenantId = _currentUser.TenantId;
        var entries = flatId.HasValue
            ? await _ledgerRepository.GetByFlatAsync(flatId.Value)
            : await _ledgerRepository.GetAllByTenantAsync(tenantId!.Value);

        return entries.Where(e => e.TenantId == tenantId).Select(e => new LedgerEntryDto
        {
            Id = e.Id, TransactionDate = e.TransactionDate, ReferenceType = e.ReferenceType,
            Description = e.Description, Debit = e.Debit, Credit = e.Credit,
            FlatId = e.FlatId, FlatNumber = e.Flat?.FlatNumber ?? ""
        }).ToList();
    }

    public async Task<TrialBalanceDto> GetTrialBalanceAsync()
    {
        var entries = await _ledgerRepository.GetAllByTenantAsync(_currentUser.TenantId!.Value);
        var totalDebit = entries.Sum(e => e.Debit);
        var totalCredit = entries.Sum(e => e.Credit);

        return new TrialBalanceDto
        {
            TotalDebit = totalDebit, TotalCredit = totalCredit,
            IsBalanced = totalDebit == totalCredit,
            Entries = entries.Select(e => new LedgerEntryDto
            {
                Id = e.Id, TransactionDate = e.TransactionDate, ReferenceType = e.ReferenceType,
                Description = e.Description, Debit = e.Debit, Credit = e.Credit,
                FlatId = e.FlatId
            }).ToList()
        };
    }

    public async Task<List<LedgerEntryDto>> GetMemberLedgerAsync(Guid flatId)
    {
        var entries = await _ledgerRepository.GetByFlatAsync(flatId);
        return entries.Select(e => new LedgerEntryDto
        {
            Id = e.Id, TransactionDate = e.TransactionDate, ReferenceType = e.ReferenceType,
            Description = e.Description, Debit = e.Debit, Credit = e.Credit,
            FlatId = e.FlatId
        }).ToList();
    }
}
