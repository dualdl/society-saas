using SocietySaaS.Domain.Entities;

namespace SocietySaaS.Domain.Interfaces;

public interface IOpeningBalanceRepository : IRepository<OpeningBalance>
{
    Task<OpeningBalance?> GetByFlatAsync(Guid flatId);
    Task<IReadOnlyList<OpeningBalance>> GetAllWithFlatAsync();
}
