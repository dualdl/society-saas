using SocietySaaS.Domain.Entities;

namespace SocietySaaS.Domain.Interfaces;

public interface IEmailQueueRepository : IRepository<EmailQueue>
{
    Task<IReadOnlyList<EmailQueue>> GetPendingEmailsAsync(int batchSize = 50);
}
