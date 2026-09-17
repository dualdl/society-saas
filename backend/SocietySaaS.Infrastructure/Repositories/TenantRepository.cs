using Microsoft.EntityFrameworkCore;
using SocietySaaS.Application.Common.Interfaces;
using SocietySaaS.Domain.Entities;
using SocietySaaS.Domain.Interfaces;

namespace SocietySaaS.Infrastructure.Repositories;

public class TenantRepository : Repository<Tenant>, ITenantRepository
{
    public TenantRepository(IApplicationDbContext context) : base(context) { }

    public async Task<Tenant?> GetWithDetailsAsync(Guid id)
    {
        return await _context.Set<Tenant>()
            .Include(t => t.Wings.Where(w => !w.IsDeleted))
            .Include(t => t.Flats.Where(f => !f.IsDeleted))
            .Include(t => t.Members.Where(m => !m.IsDeleted))
            .Include(t => t.Charges.Where(c => !c.IsDeleted))
            .FirstOrDefaultAsync(t => t.Id == id);
    }
}
