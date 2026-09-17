using SocietySaaS.Domain.Entities;

namespace SocietySaaS.Domain.Interfaces;

public interface IBillRepository : IRepository<Bill>
{
    Task<IReadOnlyList<Bill>> GetAllWithDetailsAsync();
    Task<Bill?> GetWithDetailsAsync(Guid id);
    Task<IReadOnlyList<Bill>> GetByFlatAsync(Guid flatId);
    Task<IReadOnlyList<Bill>> GetByPeriodAsync(string billingPeriod);
}
