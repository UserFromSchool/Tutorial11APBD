namespace Tutorial11.DTOs;

public class NewPrescriptionRequest
{
    public List<MedicamentRequest> Medicaments { get; set; } 
    public PatientRequest Patient { get; set; }
    public DateTime Date { get; set; }
    public DateTime DueDate { get; set; }
    public int DoctorId { get; set; }
}