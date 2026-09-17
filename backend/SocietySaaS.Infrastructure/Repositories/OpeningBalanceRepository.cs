using Microsoft.EntityFrameworkCore;
using SocietySaaS.Application.Common.Interfaces;
using SocietySaaS.Domain.Entities;
using SocietySaaS.Domain.Interfaces;

namespace SocietySaaS.Infrastructure.Repositories;

public class OpeningBalanceRepository : Repository<OpeningBalance>, IOpeningBalanceRepository
{
    public OpeningBalanceRepository(IApplicationDbContext context) : base(context) { }

    public async Task<OpeningBalance?> GetByFlatAsync(Guid flatId)
    {
        return await _dbSet.FirstOrDefaultAsync(ob => ob.FlatId == flatId);
    }

    public async Task<IReadOnlyList<OpeningBalance>> GetAllWithFlatAsync()
    {
        return await _dbSet
            .Include(ob => ob.Flat)
            .ToListAsync();
    }
}
