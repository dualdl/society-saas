using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SocietySaaS.Application.Common.Interfaces;
using SocietySaaS.Domain.Common;
using SocietySaaS.Domain.Entities;

namespace SocietySaaS.Infrastructure.Persistence;

public class AuditSaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUserService _currentUser;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuditSaveChangesInterceptor(ICurrentUserService currentUser, IHttpContextAccessor httpContextAccessor)
    {
        _currentUser = currentUser;
        _httpContextAccessor = httpContextAccessor;
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;
        if (context != null)
        {
            var auditEntries = new List<AuditLog>();
            foreach (var entry in context.ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.State == EntityState.Detached || entry.State == EntityState.Unchanged) continue;

                var auditEntry = new AuditLog
                {
                    EntityType = entry.Entity.GetType().Name,
                    EntityId = entry.Entity.Id,
                    UserId = _currentUser.UserId,
                    TenantId = _currentUser.TenantId ?? Guid.Empty,
                    Timestamp = DateTime.UtcNow,
                    IPAddress = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString(),
                    UserAgent = _httpContextAccessor.HttpContext?.Request?.Headers["User-Agent"].ToString(),
                    CorrelationId = _httpContextAccessor.HttpContext?.TraceIdentifier
                };

                switch (entry.State)
                {
                    case EntityState.Added:
                        auditEntry.Action = "Created";
                        auditEntry.Module = entry.Entity.GetType().Name;
                        auditEntry.NewValue = JsonSerializer.Serialize(entry.Entity);
                        break;
                    case EntityState.Modified:
                        auditEntry.Action = "Updated";
                        auditEntry.Module = entry.Entity.GetType().Name;
                        auditEntry.OldValue = JsonSerializer.Serialize(GetOriginalValues(entry));
                        auditEntry.NewValue = JsonSerializer.Serialize(GetCurrentValues(entry));
                        break;
                    case EntityState.Deleted:
                        auditEntry.Action = "Deleted";
                        auditEntry.Module = entry.Entity.GetType().Name;
                        auditEntry.OldValue = JsonSerializer.Serialize(entry.Entity);
                        break;
                }

                auditEntries.Add(auditEntry);
            }

            if (auditEntries.Any())
            {
                await context.Set<AuditLog>().AddRangeAsync(auditEntries, cancellationToken);
            }
        }

        return result;
    }

    private static Dictionary<string, object?> GetOriginalValues(EntityEntry entry)
    {
        var values = new Dictionary<string, object?>();
        foreach (var property in entry.Properties)
        {
            if (property.IsModified)
                values[property.Metadata.Name] = property.OriginalValue;
        }
        return values;
    }

    private static Dictionary<string, object?> GetCurrentValues(EntityEntry entry)
    {
        var values = new Dictionary<string, object?>();
        foreach (var property in entry.Properties)
        {
            if (property.IsModified)
                values[property.Metadata.Name] = property.CurrentValue;
        }
        return values;
    }
}
