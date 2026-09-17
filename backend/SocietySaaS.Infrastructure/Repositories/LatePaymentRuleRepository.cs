using Microsoft.EntityFrameworkCore;
using SocietySaaS.Application.Common.Interfaces;
using SocietySaaS.Domain.Entities;
using SocietySaaS.Domain.Interfaces;

namespace SocietySaaS.Infrastructure.Repositories;

public class LatePaymentRuleRepository : Repository<LatePaymentRule>, ILatePaymentRuleRepository
{
    public LatePaymentRuleRepository(IApplicationDbContext context) : base(context) { }

    public async Task<LatePaymentRule?> GetForTenantAsync(Guid tenantId)
    {
        return await _dbSet.FirstOrDefaultAsync(l => l.TenantId == tenantId);
    }
}
