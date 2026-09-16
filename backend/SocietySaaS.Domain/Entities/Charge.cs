using System.ComponentModel.DataAnnotations;

namespace SocietySaaS.Domain.Entities;

public class Charge : Common.BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? Description { get; set; }
    
    [MaxLength(20)]
    public string CalculationType { get; set; } = "Fixed";
    
    public decimal Amount { get; set; }
    
    public bool IsRecurring { get; set; } = true;
    
    public bool IsActive { get; set; } = true;
    
    public Tenant Tenant { get; set; } = null!;
    
    public ICollection<BillLine> BillLines { get; set; } = new List<BillLine>();
}
