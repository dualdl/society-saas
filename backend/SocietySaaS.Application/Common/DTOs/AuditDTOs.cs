namespace SocietySaaS.Application.Common.DTOs;

public record AuditLogDto
{
    public Guid Id { get; init; }
    public string Action { get; init; } = string.Empty;
    public string Module { get; init; } = string.Empty;
    public string EntityType { get; init; } = string.Empty;
    public Guid EntityId { get; init; }
    public string? OldValue { get; init; }
    public string? NewValue { get; init; }
    public DateTime Timestamp { get; init; }
    public string? UserId { get; init; }
    public Guid? TenantId { get; init; }
}

public record AuditFilterRequest
{
    public string? EntityType { get; init; }
    public Guid? EntityId { get; init; }
    public string? UserId { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
    public int Limit { get; init; } = 100;
}
