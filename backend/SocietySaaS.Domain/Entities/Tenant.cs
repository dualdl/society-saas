using System.ComponentModel.DataAnnotations;

namespace SocietySaaS.Domain.Entities;

public class Tenant
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? Address { get; set; }
    
    [MaxLength(200)]
    public string? City { get; set; }
    
    [MaxLength(100)]
    public string? State { get; set; }
    
    [MaxLength(10)]
    public string? PinCode { get; set; }
    
    [MaxLength(20)]
    public string? Phone { get; set; }
    
    [MaxLength(100)]
    public string? Email { get; set; }
    
    [MaxLength(500)]
    public string? LogoUrl { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public bool IsDeleted { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }
    
    public DateTime? DeletedAt { get; set; }
    
    public ICollection< Wing> Wings { get; set; } = new List<Wing>();
    
    public ICollection< Flat> Flats { get; set; } = new List<Flat>();
    
    public ICollection< Member> Members { get; set; } = new List<Member>();
    
    public ICollection<Charge> Charges { get; set; } = new List<Charge>();
    
    public ICollection<Bill> Bills { get; set; } = new List<Bill>();
    
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    
    public ICollection<Receipt> Receipts { get; set; } = new List<Receipt>();
    
    public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
}
