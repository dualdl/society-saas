using System.ComponentModel.DataAnnotations;

namespace SocietySaaS.Domain.Entities;

public class EmailQueue : Common.BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string TemplateName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(200)]
    public string ToEmail { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(200)]
    public string Subject { get; set; } = string.Empty;
    
    [Required]
    public string Body { get; set; } = string.Empty;
    
    [MaxLength(20)]
    public string Status { get; set; } = "Pending";
    
    public int RetryCount { get; set; }
    
    public DateTime? SentAt { get; set; }
    
    public string? ErrorMessage { get; set; }
}
