using Microsoft.EntityFrameworkCore;
using SocietySaaS.Application.Common.Interfaces;
using SocietySaaS.Domain.Entities;
using SocietySaaS.Domain.Interfaces;

namespace SocietySaaS.Infrastructure.Repositories;

public class BillRepository : Repository<Bill>, IBillRepository
{
    public BillRepository(IApplicationDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Bill>> GetAllWithDetailsAsync()
    {
        return await _dbSet
            .Where(b => !b.IsDeleted)
            .Include(b => b.Flat).ThenInclude(f => f.Wing)
            .Include(b => b.BillLines).ThenInclude(bl => bl.Charge)
            .Include(b => b.PaymentAllocations)
            .OrderByDescending(b => b.BillDate)
            .ToListAsync();
    }

    public async Task<Bill?> GetWithDetailsAsync(Guid id)
    {
        return await _dbSet
            .Include(b => b.Flat).ThenInclude(f => f.Wing)
            .Include(b => b.BillLines).ThenInclude(bl => bl.Charge)
            .Include(b => b.PaymentAllocations)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<IReadOnlyList<Bill>> GetByFlatAsync(Guid flatId)
    {
        return await _dbSet
            .Where(b => b.FlatId == flatId && !b.IsDeleted)
            .Include(b => b.BillLines).ThenInclude(bl => bl.Charge)
            .OrderByDescending(b => b.BillDate)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Bill>> GetByPeriodAsync(string billingPeriod)
    {
        return await _dbSet
            .Where(b => b.BillingPeriod == billingPeriod && !b.IsDeleted)
            .Include(b => b.Flat)
            .Include(b => b.BillLines)
            .ToListAsync();
    }
}
