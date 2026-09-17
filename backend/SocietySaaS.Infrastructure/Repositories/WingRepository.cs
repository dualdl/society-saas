using Microsoft.EntityFrameworkCore;
using SocietySaaS.Application.Common.Interfaces;
using SocietySaaS.Domain.Entities;
using SocietySaaS.Domain.Interfaces;

namespace SocietySaaS.Infrastructure.Repositories;

public class WingRepository : Repository<Wing>, IWingRepository
{
    public WingRepository(IApplicationDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Wing>> GetAllActiveAsync()
    {
        return await _dbSet.Where(w => w.IsActive && !w.IsDeleted).ToListAsync();
    }
}
