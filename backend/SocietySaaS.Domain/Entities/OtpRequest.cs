using System.ComponentModel.DataAnnotations;

namespace SocietySaaS.Domain.Entities;

public class OtpRequest
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    [MaxLength(200)]
    public string Target { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(10)]
    public string Code { get; set; } = string.Empty;
    
    [MaxLength(50)]
    public string Purpose { get; set; } = "Login";
    
    public DateTime ExpiresAt { get; set; }
    
    public bool IsUsed { get; set; }
    
    public int AttemptCount { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
