using SocietySaaS.Domain.Entities;

namespace SocietySaaS.Domain.Interfaces;

public interface IPaymentRepository : IRepository<Payment>
{
    Task<IReadOnlyList<Payment>> GetAllWithDetailsAsync();
    Task<Payment?> GetWithDetailsAsync(Guid id);
}
