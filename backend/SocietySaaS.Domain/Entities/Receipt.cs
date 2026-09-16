using System.ComponentModel.DataAnnotations;

namespace SocietySaaS.Domain.Entities;

public class Receipt : Common.BaseEntity
{
    [Required]
    [MaxLength(50)]
    public string ReceiptNumber { get; set; } = string.Empty;
    
    public DateTime ReceiptDate { get; set; }
    
    public decimal Amount { get; set; }
    
    [MaxLength(20)]
    public string PaymentMode { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string? TransactionReference { get; set; }
    
    [MaxLength(500)]
    public string? PdfUrl { get; set; }
    
    public Guid PaymentId { get; set; }
    
    public Payment Payment { get; set; } = null!;
    
    public Guid FlatId { get; set; }
    
    public Flat Flat { get; set; } = null!;
    
    public Tenant Tenant { get; set; } = null!;
}
