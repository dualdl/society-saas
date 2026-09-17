using Microsoft.EntityFrameworkCore;
using SocietySaaS.Application.Common.Interfaces;
using SocietySaaS.Domain.Entities;
using SocietySaaS.Domain.Interfaces;

namespace SocietySaaS.Infrastructure.Repositories;

public class MemberRepository : Repository<Member>, IMemberRepository
{
    public MemberRepository(IApplicationDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Member>> GetAllWithFlatAsync()
    {
        return await _dbSet
            .Where(m => !m.IsDeleted)
            .Include(m => m.Flat)
            .ToListAsync();
    }

    public async Task<Member?> GetWithFlatAsync(Guid id)
    {
        return await _dbSet
            .Include(m => m.Flat)
            .FirstOrDefaultAsync(m => m.Id == id);
    }
}
