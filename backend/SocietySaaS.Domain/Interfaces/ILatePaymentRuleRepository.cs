using SocietySaaS.Domain.Entities;

namespace SocietySaaS.Domain.Interfaces;

public interface ILatePaymentRuleRepository : IRepository<LatePaymentRule>
{
    Task<LatePaymentRule?> GetForTenantAsync(Guid tenantId);
}
