using SocietySaaS.Domain.Entities;

namespace SocietySaaS.Domain.Interfaces;

public interface IFlatRepository : IRepository<Flat>
{
    Task<IReadOnlyList<Flat>> GetAllWithMembersAsync();
    Task<Flat?> GetWithDetailsAsync(Guid id);
    Task<IReadOnlyList<Flat>> GetByWingAsync(Guid wingId);
}
