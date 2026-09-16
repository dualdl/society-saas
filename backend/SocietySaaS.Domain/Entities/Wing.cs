using System.ComponentModel.DataAnnotations;

namespace SocietySaaS.Domain.Entities;

public class Wing : Common.BaseEntity
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    
    public int TotalFloors { get; set; }
    
    public int FlatsPerFloor { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public Tenant Tenant { get; set; } = null!;
    
    public ICollection<Flat> Flats { get; set; } = new List<Flat>();
}
