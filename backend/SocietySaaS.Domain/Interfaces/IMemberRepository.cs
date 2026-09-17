using SocietySaaS.Domain.Entities;

namespace SocietySaaS.Domain.Interfaces;

public interface IMemberRepository : IRepository<Member>
{
    Task<IReadOnlyList<Member>> GetAllWithFlatAsync();
    Task<Member?> GetWithFlatAsync(Guid id);
}
