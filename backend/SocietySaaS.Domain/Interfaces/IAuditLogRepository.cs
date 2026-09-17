using SocietySaaS.Domain.Entities;

namespace SocietySaaS.Domain.Interfaces;

public interface IAuditLogRepository : IRepository<AuditLog>
{
    Task<IReadOnlyList<AuditLog>> GetByEntityAsync(string entityType, Guid entityId);
    Task<IReadOnlyList<AuditLog>> GetByUserAsync(string userId);
}
