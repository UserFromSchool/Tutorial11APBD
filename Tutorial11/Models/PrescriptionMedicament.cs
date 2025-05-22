using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Tutorial11.Models;

[PrimaryKey(nameof(Prescription))]
public class PrescriptionMedicament
{
    [ForeignKey(nameof(Prescription))]
    public int IdPrescription { get; set; }
    
    [ForeignKey(nameof(Medicament))]
    public int IdMedicament { get; set; }
    
    public int? Dose { get; set; }
    
    [MaxLength(100)]
    public string Details { get; set; } 
    
    public Prescription Prescription { get; set; }
    
    public Medicament Medicament { get; set; }
}