using System.ComponentModel.DataAnnotations;

namespace SocietySaaS.Domain.Entities;

public class AuditLog
{
    [Key]
    public Guid Id { get; set; }
    
    public Guid TenantId { get; set; }
    
    public Guid? UserId { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Action { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string Module { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string EntityType { get; set; } = string.Empty;
    
    public Guid? EntityId { get; set; }
    
    [MaxLength(4000)]
    public string? OldValue { get; set; }
    
    [MaxLength(4000)]
    public string? NewValue { get; set; }
    
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    [MaxLength(50)]
    public string? IPAddress { get; set; }
    
    [MaxLength(500)]
    public string? UserAgent { get; set; }
    
    [MaxLength(100)]
    public string? CorrelationId { get; set; }
}
