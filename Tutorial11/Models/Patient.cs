using System.ComponentModel.DataAnnotations;

namespace Tutorial11.Models;

public class Patient
{
    
    [Key]
    public int IdPatient { get; set; }
    
    [MaxLength(100)]
    public string FirstName { get; set; }
    
    [MaxLength(100)]
    public string LastName { get; set; }
    
    public DateTime Birthday { get; set; }
    
    public ICollection<Prescription> Prescriptions { get; set; }
    
}