using System.ComponentModel.DataAnnotations;

namespace Tutorial11.Models;

public class Doctor
{
    [Key]
    public required int IdDoctor { get; set; }
    
    [MaxLength(100)]
    public required string FirstName { get; set; }
    
    [MaxLength(100)]
    public required string LastName { get; set; }
    
    [MaxLength(100)]
    public required string Email { get; set; }
    
    public required ICollection<Prescription> Prescriptions { get; set; }
    
}