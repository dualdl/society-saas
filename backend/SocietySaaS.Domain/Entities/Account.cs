using System.ComponentModel.DataAnnotations;
using SocietySaaS.Domain.Common;

namespace SocietySaaS.Domain.Entities;

public class Account : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(50)]
    public string AccountType { get; set; } = string.Empty;
    
    public bool IsActive { get; set; } = true;
}
