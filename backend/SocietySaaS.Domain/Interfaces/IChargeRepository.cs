using SocietySaaS.Domain.Entities;

namespace SocietySaaS.Domain.Interfaces;

public interface IChargeRepository : IRepository<Charge>
{
    Task<IReadOnlyList<Charge>> GetActiveChargesAsync();
}
