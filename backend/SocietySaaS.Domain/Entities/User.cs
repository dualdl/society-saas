using System.ComponentModel.DataAnnotations;

namespace SocietySaaS.Domain.Entities;

public class User
{
    [Key]
    public Guid Id { get; set; }
    
    [MaxLength(100)]
    public string? FirstName { get; set; }
    
    [MaxLength(100)]
    public string? LastName { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;
    
    [MaxLength(20)]
    public string? Mobile { get; set; }
    
    [MaxLength(500)]
    public string? PasswordHash { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public bool IsSuperAdmin { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? LastLoginAt { get; set; }
    
    public ICollection<UserTenant> UserTenants { get; set; } = new List<UserTenant>();
}
