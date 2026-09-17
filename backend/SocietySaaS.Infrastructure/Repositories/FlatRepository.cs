using Microsoft.EntityFrameworkCore;
using SocietySaaS.Application.Common.Interfaces;
using SocietySaaS.Domain.Entities;
using SocietySaaS.Domain.Interfaces;

namespace SocietySaaS.Infrastructure.Repositories;

public class FlatRepository : Repository<Flat>, IFlatRepository
{
    public FlatRepository(IApplicationDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Flat>> GetAllWithMembersAsync()
    {
        return await _dbSet
            .Where(f => !f.IsDeleted)
            .Include(f => f.Wing)
            .Include(f => f.Members.Where(m => !m.IsDeleted))
            .OrderBy(f => f.FlatNumber)
            .ToListAsync();
    }

    public async Task<Flat?> GetWithDetailsAsync(Guid id)
    {
        return await _dbSet
            .Include(f => f.Wing)
            .Include(f => f.Members.Where(m => !m.IsDeleted))
            .Include(f => f.Bills.Where(b => !b.IsDeleted).OrderByDescending(b => b.BillDate).Take(10))
            .FirstOrDefaultAsync(f => f.Id == id);
    }

    public async Task<IReadOnlyList<Flat>> GetByWingAsync(Guid wingId)
    {
        return await _dbSet
            .Where(f => f.WingId == wingId && !f.IsDeleted)
            .Include(f => f.Members.Where(m => !m.IsDeleted))
            .ToListAsync();
    }
}
