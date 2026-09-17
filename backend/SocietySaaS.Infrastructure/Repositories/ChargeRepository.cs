using Microsoft.EntityFrameworkCore;
using SocietySaaS.Application.Common.Interfaces;
using SocietySaaS.Domain.Entities;
using SocietySaaS.Domain.Interfaces;

namespace SocietySaaS.Infrastructure.Repositories;

public class ChargeRepository : Repository<Charge>, IChargeRepository
{
    public ChargeRepository(IApplicationDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Charge>> GetActiveChargesAsync()
    {
        return await _dbSet.Where(c => c.IsActive && !c.IsDeleted).ToListAsync();
    }
}
