using Microsoft.EntityFrameworkCore;
using SocietySaaS.Application.Common.Interfaces;
using SocietySaaS.Domain.Entities;
using SocietySaaS.Domain.Interfaces;

namespace SocietySaaS.Infrastructure.Repositories;

public class LedgerEntryRepository : ILedgerEntryRepository
{
    private readonly IApplicationDbContext _context;

    public LedgerEntryRepository(IApplicationDbContext context) => _context = context;

    public async Task<IReadOnlyList<LedgerEntry>> GetAllByTenantAsync(Guid tenantId)
    {
        return await _context.Set<LedgerEntry>()
            .Where(l => l.TenantId == tenantId)
            .Include(l => l.Flat)
            .OrderByDescending(l => l.TransactionDate)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<LedgerEntry>> GetByFlatAsync(Guid flatId)
    {
        return await _context.Set<LedgerEntry>()
            .Where(l => l.FlatId == flatId)
            .OrderByDescending(l => l.TransactionDate)
            .ToListAsync();
    }

    public async Task AddAsync(LedgerEntry entry)
    {
        await _context.Set<LedgerEntry>().AddAsync(entry);
    }

    public async Task AddRangeAsync(IEnumerable<LedgerEntry> entries)
    {
        await _context.Set<LedgerEntry>().AddRangeAsync(entries);
    }
}
