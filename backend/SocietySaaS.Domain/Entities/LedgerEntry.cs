using System.ComponentModel.DataAnnotations;
using SocietySaaS.Domain.Common;

namespace SocietySaaS.Domain.Entities;

public class LedgerEntry : BaseEntity
{
    public DateTime TransactionDate { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string ReferenceType { get; set; } = string.Empty;
    
    public Guid? ReferenceId { get; set; }
    
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;
    
    public decimal Debit { get; set; }
    
    public decimal Credit { get; set; }
    
    public decimal Balance => Debit - Credit;
    
    public Guid FlatId { get; set; }
    
    public Flat Flat { get; set; } = null!;
}
