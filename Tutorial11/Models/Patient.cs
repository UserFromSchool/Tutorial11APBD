using System.ComponentModel.DataAnnotations;

namespace Tutorial11.Models;

public class Patient
{
    
    [Key]
    public required int IdPatient { get; set; }
    
    [MaxLength(100)]
    public required string FirstName { get; set; }
    
    [MaxLength(100)]
    public required string LastName { get; set; }
    
    [MaxLength(100)]
    public required DateTime Birthday { get; set; }
    
    public required ICollection<Prescription> Prescriptions { get; set; }
    
}