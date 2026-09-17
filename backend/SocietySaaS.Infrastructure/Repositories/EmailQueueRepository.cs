using Microsoft.EntityFrameworkCore;
using SocietySaaS.Application.Common.Interfaces;
using SocietySaaS.Domain.Entities;
using SocietySaaS.Domain.Interfaces;

namespace SocietySaaS.Infrastructure.Repositories;

public class EmailQueueRepository : Repository<EmailQueue>, IEmailQueueRepository
{
    public EmailQueueRepository(IApplicationDbContext context) : base(context) { }

    public async Task<IReadOnlyList<EmailQueue>> GetPendingEmailsAsync(int batchSize = 50)
    {
        return await _dbSet
            .Where(e => e.Status == "Pending" && e.RetryCount < 3)
            .OrderBy(e => e.CreatedAt)
            .Take(batchSize)
            .ToListAsync();
    }
}
