using Microsoft.EntityFrameworkCore;
using SocietySaaS.Application.Common.Interfaces;
using SocietySaaS.Domain.Entities;
using SocietySaaS.Domain.Interfaces;

namespace SocietySaaS.Infrastructure.Repositories;

public class PaymentRepository : Repository<Payment>, IPaymentRepository
{
    public PaymentRepository(IApplicationDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Payment>> GetAllWithDetailsAsync()
    {
        return await _dbSet
            .Where(p => !p.IsDeleted)
            .Include(p => p.Flat).ThenInclude(f => f.Wing)
            .Include(p => p.PaymentAllocations).ThenInclude(pa => pa.Bill)
            .Include(p => p.Receipt)
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync();
    }

    public async Task<Payment?> GetWithDetailsAsync(Guid id)
    {
        return await _dbSet
            .Include(p => p.Flat).ThenInclude(f => f.Wing)
            .Include(p => p.PaymentAllocations).ThenInclude(pa => pa.Bill)
            .Include(p => p.Receipt)
            .FirstOrDefaultAsync(p => p.Id == id);
    }
}
