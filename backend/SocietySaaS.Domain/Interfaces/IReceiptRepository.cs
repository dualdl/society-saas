using SocietySaaS.Domain.Entities;

namespace SocietySaaS.Domain.Interfaces;

public interface IReceiptRepository : IRepository<Receipt>
{
    Task<IReadOnlyList<Receipt>> GetAllWithDetailsAsync();
    Task<Receipt?> GetWithDetailsAsync(Guid id);
}
