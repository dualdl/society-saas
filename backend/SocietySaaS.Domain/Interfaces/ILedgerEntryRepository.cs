using SocietySaaS.Domain.Entities;

namespace SocietySaaS.Domain.Interfaces;

public interface ILedgerEntryRepository
{
    Task<IReadOnlyList<LedgerEntry>> GetAllByTenantAsync(Guid tenantId);
    Task<IReadOnlyList<LedgerEntry>> GetByFlatAsync(Guid flatId);
    Task AddAsync(LedgerEntry entry);
    Task AddRangeAsync(IEnumerable<LedgerEntry> entries);
}
