using System.ComponentModel.DataAnnotations;

namespace SocietySaaS.Domain.Entities;

public class ImportJob
{
    [Key]
    public Guid Id { get; set; }
    
    public Guid TenantId { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string JobType { get; set; } = string.Empty;
    
    [MaxLength(200)]
    public string? FileName { get; set; }
    
    [MaxLength(500)]
    public string? BlobUrl { get; set; }
    
    [MaxLength(20)]
    public string Status { get; set; } = "Pending";
    
    public int TotalRows { get; set; }
    
    public int ProcessedRows { get; set; }
    
    public int ErrorRows { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? CompletedAt { get; set; }
    
    public string? ErrorMessage { get; set; }
    
    public ICollection<ImportRow> ImportRows { get; set; } = new List<ImportRow>();
}
