using System.ComponentModel.DataAnnotations;

namespace SocietySaaS.Domain.Entities;

public class PaymentAllocation : Common.BaseEntity
{
    public decimal Amount { get; set; }
    
    public Guid PaymentId { get; set; }
    
    public Payment Payment { get; set; } = null!;
    
    public Guid BillId { get; set; }
    
    public Bill Bill { get; set; } = null!;
}
