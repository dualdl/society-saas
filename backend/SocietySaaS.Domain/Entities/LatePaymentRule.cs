using System.ComponentModel.DataAnnotations;

namespace SocietySaaS.Domain.Entities;

public class LatePaymentRule : Common.BaseEntity
{
    public bool IsEnabled { get; set; }
    
    public decimal Percentage { get; set; }
    
    public int GracePeriodDays { get; set; }
    
    [MaxLength(50)]
    public string CalculationType { get; set; } = "OutstandingPrincipal";
    
    [MaxLength(20)]
    public string Frequency { get; set; } = "Monthly";
    
    public decimal? MaximumFine { get; set; }
}
