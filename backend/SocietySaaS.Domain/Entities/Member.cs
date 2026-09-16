using System.ComponentModel.DataAnnotations;

namespace SocietySaaS.Domain.Entities;

public class Member : Common.BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string? LastName { get; set; }
    
    [Required]
    [MaxLength(20)]
    public string Mobile { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string? Email { get; set; }
    
    [MaxLength(20)]
    public string MemberType { get; set; } = "Owner";
    
    public bool IsPrimary { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public Guid FlatId { get; set; }
    
    public Flat Flat { get; set; } = null!;
    
    public Tenant Tenant { get; set; } = null!;
}
