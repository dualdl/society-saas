using System.ComponentModel.DataAnnotations;

namespace SocietySaaS.Domain.Entities;

public class ImportRow
{
    [Key]
    public Guid Id { get; set; }
    
    public int RowNumber { get; set; }
    
    [MaxLength(4000)]
    public string? RawData { get; set; }
    
    [MaxLength(20)]
    public string Status { get; set; } = "Pending";
    
    [MaxLength(4000)]
    public string? ErrorMessage { get; set; }
    
    public Guid ImportJobId { get; set; }
    
    public ImportJob ImportJob { get; set; } = null!;
}
