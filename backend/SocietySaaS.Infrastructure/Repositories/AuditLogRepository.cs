using Microsoft.EntityFrameworkCore;
using SocietySaaS.Application.Common.Interfaces;
using SocietySaaS.Domain.Entities;
using SocietySaaS.Domain.Interfaces;

namespace SocietySaaS.Infrastructure.Repositories;

public class AuditLogRepository : Repository<AuditLog>, IAuditLogRepository
{
    public AuditLogRepository(IApplicationDbContext context) : base(context) { }

    public async Task<IReadOnlyList<AuditLog>> GetByEntityAsync(string entityType, Guid entityId)
    {
        return await _dbSet
            .Where(a => a.EntityType == entityType && a.EntityId == entityId)
            .OrderByDescending(a => a.Timestamp)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<AuditLog>> GetByUserAsync(string userId)
    {
        if (!Guid.TryParse(userId, out var userGuid))
            return new List<AuditLog>();

        return await _dbSet
            .Where(a => a.UserId == userGuid)
            .OrderByDescending(a => a.Timestamp)
            .ToListAsync();
    }
}
