
using Microsoft.AspNetCore.Mvc;
using Tutorial11.DTOs;
using Tutorial11.Services;

namespace Tutorial11.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ApiController(IHospitalService service) : Controller
{

    private readonly IHospitalService _service = service;

    [HttpPost("/prescriptions")]
    public async Task<IActionResult> AddPrescription([FromBody] NewPrescriptionRequest request)
    {
        var response = await _service.AddPrescription(request);
        return response.Status switch
        {
            200 => Ok(response.Message),
            400 => BadRequest(response.Message),
            500 => StatusCode(500, response.Message),
            _ => StatusCode(500, "Unexpected server processing occured.")
        };
    }

    [HttpGet("/patients")]
    public async Task<IActionResult> GetPatients(int id)
    {
        var response = await _service.GetPatientInfo(id);
        return response.Status switch
        {
            200 => Ok(response.PatientInfo),
            400 => BadRequest(response.Message),
            500 => StatusCode(500, response.Message),
            _ => StatusCode(500, "Unexpected server processing occured.")
        };
    }
    
}