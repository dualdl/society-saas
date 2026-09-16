using System.ComponentModel.DataAnnotations;

namespace SocietySaaS.Domain.Entities;

public class Document
{
    [Key]
    public Guid Id { get; set; }
    
    public Guid TenantId { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string? Category { get; set; }
    
    [MaxLength(500)]
    public string? BlobUrl { get; set; }
    
    [MaxLength(100)]
    public string? ContentType { get; set; }
    
    public long FileSize { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public string? CreatedBy { get; set; }
}
