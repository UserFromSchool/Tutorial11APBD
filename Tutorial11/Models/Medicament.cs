using System.ComponentModel.DataAnnotations;

namespace Tutorial11.Models;

public class Medicament
{
    
    [Key]
    public required int IdMedicament { get; set; }
    
    [MaxLength(100)]
    public required string Name { get; set; }
    
    [MaxLength(100)]
    public required string Description { get; set; }
    
    [MaxLength(100)]
    public required string Type { get; set; }
    
    public required ICollection<PrescriptionMedicament> PrescriptionMedicaments { get; set; }
    
}