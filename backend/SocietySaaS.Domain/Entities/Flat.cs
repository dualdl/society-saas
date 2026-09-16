using System.ComponentModel.DataAnnotations;

namespace SocietySaaS.Domain.Entities;

public class Flat : Common.BaseEntity
{
    [Required]
    [MaxLength(50)]
    public string FlatNumber { get; set; } = string.Empty;
    
    public int Floor { get; set; }
    
    public decimal CarpetArea { get; set; }
    
    public decimal BuiltUpArea { get; set; }
    
    [MaxLength(50)]
    public string? FlatType { get; set; }
    
    [MaxLength(20)]
    public string OccupancyStatus { get; set; } = "Owner";
    
    public bool IsActive { get; set; } = true;
    
    public Guid? WingId { get; set; }
    
    public Wing? Wing { get; set; }
    
    public Tenant Tenant { get; set; } = null!;
    
    public ICollection<Member> Members { get; set; } = new List<Member>();
    
    public ICollection<Bill> Bills { get; set; } = new List<Bill>();
    
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    
    public ICollection<Receipt> Receipts { get; set; } = new List<Receipt>();
    
    public OpeningBalance? OpeningBalance { get; set; }
}
