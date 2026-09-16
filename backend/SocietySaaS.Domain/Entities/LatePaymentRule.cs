using System.ComponentModel.DataAnnotations;

namespace SocietySaaS.Domain.Entities;

public class LatePaymentRule
{
    [Key]
    public Guid Id { get; set; }
    
    public Guid TenantId { get; set; }
    
    public bool IsEnabled { get; set; }
    
    public decimal Percentage { get; set; }
    
    public int GracePeriodDays { get; set; }
    
    [MaxLength(50)]
    public string CalculationType { get; set; } = "OutstandingPrincipal";
    
    [MaxLength(20)]
    public string Frequency { get; set; } = "Monthly";
    
    public decimal? MaximumFine { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }
}
