using System.ComponentModel.DataAnnotations;

namespace SocietySaaS.Domain.Entities;

public class Payment : Common.BaseEntity
{
    [Required]
    [MaxLength(50)]
    public string PaymentNumber { get; set; } = string.Empty;
    
    public DateTime PaymentDate { get; set; }
    
    public decimal Amount { get; set; }
    
    [Required]
    [MaxLength(20)]
    public string PaymentMode { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string? TransactionReference { get; set; }
    
    [MaxLength(500)]
    public string? Notes { get; set; }
    
    [MaxLength(20)]
    public string Status { get; set; } = "Completed";
    
    public bool IsReversed { get; set; }
    
    public DateTime? ReversedAt { get; set; }
    
    public string? ReversedBy { get; set; }
    
    public string? ReversalReason { get; set; }
    
    public Guid FlatId { get; set; }
    
    public Flat Flat { get; set; } = null!;
    
    public Tenant Tenant { get; set; } = null!;
    
    public ICollection<PaymentAllocation> PaymentAllocations { get; set; } = new List<PaymentAllocation>();
    
    public Receipt? Receipt { get; set; }
}
