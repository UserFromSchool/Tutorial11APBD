using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tutorial11.Models;

public class Prescription
{
    
    [Key]
    public int IdPrescrription { get; set; }
    
    public DateTime Date { get; set; } 
    
    public DateTime DueDate { get; set; } 
    
    [ForeignKey(nameof(Doctor))]
    public int IdDoctor { get; set; }
    
    [ForeignKey(nameof(Patient))]
    public int IdPatient { get; set; }
    
    public Doctor Doctor { get; set; }
    public Patient Patient { get; set; }
    
    public ICollection<PrescriptionMedicament> PrescriptionMedicaments { get; set; }
    
}