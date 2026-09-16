using System.ComponentModel.DataAnnotations;

namespace SocietySaaS.Domain.Entities;

public class UserTenant
{
    [Key]
    public Guid Id { get; set; }
    
    public Guid UserId { get; set; }
    
    public User User { get; set; } = null!;
    
    public Guid TenantId { get; set; }
    
    public Tenant Tenant { get; set; } = null!;
    
    [MaxLength(50)]
    public string Role { get; set; } = string.Empty;
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
