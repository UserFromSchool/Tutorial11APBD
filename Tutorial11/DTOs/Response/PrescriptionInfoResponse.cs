namespace Tutorial11.DTOs.Response;

public class PrescriptionInfoResponse
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public DateTime DueDate { get; set; }
    public List<MedicamentInfoResponse> Medicaments { get; set; }
    public DoctorInfoResponse Doctor { get; set; }
}