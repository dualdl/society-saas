using System.ComponentModel.DataAnnotations;

namespace SocietySaaS.Domain.Common;

public abstract class BaseEntity
{
    [Key]
    public Guid Id { get; set; }
    
    public Guid TenantId { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public string? CreatedBy { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
    
    public string? UpdatedBy { get; set; }
    
    public bool IsDeleted { get; set; }
    
    public DateTime? DeletedAt { get; set; }
    
    public string? DeletedBy { get; set; }
    
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}
