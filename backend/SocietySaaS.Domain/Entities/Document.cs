using System.ComponentModel.DataAnnotations;

namespace SocietySaaS.Domain.Entities;

public class Document : Common.BaseEntity
{
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
}
