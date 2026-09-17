using SocietySaaS.Domain.Entities;

namespace SocietySaaS.Domain.Interfaces;

public interface IWingRepository : IRepository<Wing>
{
    Task<IReadOnlyList<Wing>> GetAllActiveAsync();
}
