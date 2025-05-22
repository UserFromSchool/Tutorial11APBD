using Tutorial11.DTOs.Response;
using Tutorial11.DTOs;

namespace Tutorial11.Services;

public interface IHospitalService
{

    public Task<NewPrescriptionResponse> AddPrescription(NewPrescriptionRequest request);
    
    public Task<GetPatientInfoResponse> GetPatientInfo(int id);

}