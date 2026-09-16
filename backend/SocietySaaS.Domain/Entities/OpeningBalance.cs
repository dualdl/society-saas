using System.ComponentModel.DataAnnotations;

namespace SocietySaaS.Domain.Entities;

public class OpeningBalance : Common.BaseEntity
{
    public decimal Amount { get; set; }
    
    [MaxLength(20)]
    public string BalanceType { get; set; } = "Debit";
    
    public DateTime AsOfDate { get; set; }
    
    [MaxLength(500)]
    public string? Notes { get; set; }
    
    public Guid FlatId { get; set; }
    
    public Flat Flat { get; set; } = null!;
    
    public Tenant Tenant { get; set; } = null!;
}
