using System.ComponentModel.DataAnnotations;

namespace SocietySaaS.Domain.Entities;

public class BillLine : Common.BaseEntity
{
    public decimal Amount { get; set; }
    
    [MaxLength(200)]
    public string? Description { get; set; }
    
    public Guid BillId { get; set; }
    
    public Bill Bill { get; set; } = null!;
    
    public Guid ChargeId { get; set; }
    
    public Charge Charge { get; set; } = null!;
}
