using Microsoft.EntityFrameworkCore;
using SocietySaaS.Application.Common.DTOs;
using SocietySaaS.Application.Common.Interfaces;
using SocietySaaS.Domain.Entities;
using SocietySaaS.Domain.Interfaces;

namespace SocietySaaS.Application.Services;

public interface IAuditService
{
    Task LogAsync(string action, string module, string entityType, Guid? entityId = null, string? oldValue = null, string? newValue = null);
    Task<List<AuditLog>> GetLogsAsync(string? entityType, Guid? entityId, string? userId, int limit = 100);
}

public class AuditService : IAuditService
{
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public AuditService(IAuditLogRepository auditLogRepository, IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _auditLogRepository = auditLogRepository;
        _context = context;
        _currentUser = currentUser;
    }

    public async Task LogAsync(string action, string module, string entityType, Guid? entityId = null, string? oldValue = null, string? newValue = null)
    {
        var log = new AuditLog
        {
            Id = Guid.NewGuid(),
            Action = action,
            Module = module,
            EntityType = entityType,
            EntityId = entityId,
            OldValue = oldValue,
            NewValue = newValue,
            UserId = _currentUser.UserId,
            TenantId = _currentUser.TenantId ?? Guid.Empty,
            Timestamp = DateTime.UtcNow
        };

        await _auditLogRepository.AddAsync(log);
    }

    public async Task<List<AuditLog>> GetLogsAsync(string? entityType, Guid? entityId, string? userId, int limit = 100)
    {
        var query = _context.AuditLogs.AsQueryable();

        if (!string.IsNullOrEmpty(entityType))
            query = query.Where(a => a.EntityType == entityType);

        if (entityId.HasValue)
            query = query.Where(a => a.EntityId == entityId.Value);

        if (!string.IsNullOrEmpty(userId) && Guid.TryParse(userId, out var parsedUserId))
            query = query.Where(a => a.UserId == parsedUserId);

        if (_currentUser.TenantId.HasValue)
            query = query.Where(a => a.TenantId == _currentUser.TenantId.Value);

        return await query
            .OrderByDescending(a => a.Timestamp)
            .Take(limit)
            .ToListAsync();
    }
}
