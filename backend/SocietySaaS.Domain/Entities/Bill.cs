using System.ComponentModel.DataAnnotations;

namespace SocietySaaS.Domain.Entities;

public class Bill : Common.BaseEntity
{
    [Required]
    [MaxLength(50)]
    public string BillNumber { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(20)]
    public string BillingPeriod { get; set; } = string.Empty;
    
    public DateTime BillDate { get; set; }
    
    public DateTime DueDate { get; set; }
    
    public decimal PreviousOutstanding { get; set; }
    
    public decimal CurrentCharges { get; set; }
    
    public decimal LateFee { get; set; }
    
    public decimal Adjustment { get; set; }
    
    public decimal GrandTotal { get; set; }
    
    public decimal AmountPaid { get; set; }
    
    public decimal BalanceOutstanding { get; set; }
    
    [MaxLength(20)]
    public string Status { get; set; } = "Pending";
    
    public bool IsCancelled { get; set; }
    
    public DateTime? CancelledAt { get; set; }
    
    public string? CancelledBy { get; set; }
    
    public string? CancellationReason { get; set; }
    
    public Guid FlatId { get; set; }
    
    public Flat Flat { get; set; } = null!;
    
    public Tenant Tenant { get; set; } = null!;
    
    public ICollection<BillLine> BillLines { get; set; } = new List<BillLine>();
    
    public ICollection<PaymentAllocation> PaymentAllocations { get; set; } = new List<PaymentAllocation>();
}
