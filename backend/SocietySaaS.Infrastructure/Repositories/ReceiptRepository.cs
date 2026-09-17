using Microsoft.EntityFrameworkCore;
using SocietySaaS.Application.Common.Interfaces;
using SocietySaaS.Domain.Entities;
using SocietySaaS.Domain.Interfaces;

namespace SocietySaaS.Infrastructure.Repositories;

public class ReceiptRepository : Repository<Receipt>, IReceiptRepository
{
    public ReceiptRepository(IApplicationDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Receipt>> GetAllWithDetailsAsync()
    {
        return await _dbSet
            .Where(r => !r.IsDeleted)
            .Include(r => r.Flat).ThenInclude(f => f.Wing)
            .Include(r => r.Payment)
            .OrderByDescending(r => r.ReceiptDate)
            .ToListAsync();
    }

    public async Task<Receipt?> GetWithDetailsAsync(Guid id)
    {
        return await _dbSet
            .Include(r => r.Flat).ThenInclude(f => f.Wing)
            .Include(r => r.Payment)
            .FirstOrDefaultAsync(r => r.Id == id);
    }
}
