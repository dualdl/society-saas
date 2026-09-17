using SocietySaaS.Domain.Entities;

namespace SocietySaaS.Domain.Interfaces;

public interface ITenantRepository : IRepository<Tenant>
{
    Task<Tenant?> GetWithDetailsAsync(Guid id);
}
