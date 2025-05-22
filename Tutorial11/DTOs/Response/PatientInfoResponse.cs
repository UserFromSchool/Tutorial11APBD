namespace Tutorial11.DTOs.Response;

public class PatientInfoResponse
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime Birthday { get; set; }
    public List<PrescriptionInfoResponse> Prescriptions { get; set; }
}