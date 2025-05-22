namespace Tutorial11.DTOs.Response;

public class MedicamentInfoResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Details { get; set; }
    public int? Dose { get; set; }
}