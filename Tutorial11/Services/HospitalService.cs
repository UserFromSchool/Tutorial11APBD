using Microsoft.EntityFrameworkCore;
using Tutorial11.Data;
using Tutorial11.DTOs;
using Tutorial11.DTOs.Response;
using Tutorial11.Models;

namespace Tutorial11.Services;

public class HospitalService(DatabaseContext context) : IHospitalService
{

    private readonly DatabaseContext _context = context;
    
    public async Task<NewPrescriptionResponse> AddPrescription(NewPrescriptionRequest request)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Check medicaments' amount
            if (request.Medicaments.Count > 10)
            {
                return new NewPrescriptionResponse
                    { Status = 400, Message = "Prescription has a limit of 10 medicaments." };
            }

            // Check if dates are correct
            if (request.DueDate < request.Date)
            {
                return new NewPrescriptionResponse { Status = 400, Message = "Incorrect dates." };
            }

            // Get or create patient
            Patient patient;
            if (!await _context.Patients.AnyAsync(e => e.IdPatient == request.Patient.Id))
            {
                patient = new Patient
                {
                    FirstName = request.Patient.FirstName,
                    LastName = request.Patient.LastName,
                    Birthday = request.Patient.BirthDate
                };
                _context.Patients.Add(patient);
                await _context.SaveChangesAsync();
            }
            else
            {
                patient = await _context.Patients.FirstAsync(e => e.IdPatient == request.Patient.Id);
            }

            // Verify doctor exists
            var doctorExists = await _context.Doctors.AnyAsync(e => e.IdDoctor == request.DoctorId);
            if (!doctorExists)
            {
                return new NewPrescriptionResponse { Status = 400, Message = "Doctor not found." };
            }

            // Extract medicament IDs and verify they exist
            var requestedMedicamentIds = request.Medicaments.Select(m => m.Id).ToList();
            
            var existingMedicamentIds = await _context.Medicaments
                .Where(e => requestedMedicamentIds.Contains(e.IdMedicament))
                .Select(e => e.IdMedicament)
                .ToListAsync();
                
            if (existingMedicamentIds.Count != request.Medicaments.Count)
            {
                var missingIds = requestedMedicamentIds.Except(existingMedicamentIds);
                return new NewPrescriptionResponse 
                { 
                    Status = 400, 
                    Message = $"Medicaments not found: {string.Join(", ", missingIds)}" 
                };
            }

            // Create new prescription
            var prescription = new Prescription
            {
                Date = request.Date,
                DueDate = request.DueDate,
                IdDoctor = request.DoctorId,
                IdPatient = patient.IdPatient
            };
            
            _context.Prescriptions.Add(prescription);
            await _context.SaveChangesAsync();

            // Create prescription-medicament relationships
            var prescriptionMedicaments = request.Medicaments.Select(requestMedicament => 
                new PrescriptionMedicament
                {
                    IdPrescription = prescription.IdPrescrription,
                    IdMedicament = requestMedicament.Id,
                    Details = requestMedicament.Details,
                    Dose = requestMedicament.Dose
                }).ToList();
            
            _context.PrescriptionMedicaments.AddRange(prescriptionMedicaments);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
            _context.Patients.ToList().ForEach(p => Console.WriteLine(p.IdPatient));
            return new NewPrescriptionResponse { Status = 200, Message = "Successfully added new prescription." };
        }
        catch (Exception exception)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"Error details: {exception.Message}");
            if (exception.InnerException != null)
            {
                Console.WriteLine($"Inner exception: {exception.InnerException.Message}");
            }
            
            return new NewPrescriptionResponse 
            { 
                Status = 500, 
                Message = $"Error occurred: {exception.Message}" 
            };
        }
    }

    public async Task<GetPatientInfoResponse> GetPatientInfo(int id)
    {
        try
        {
            // Get patient with all related data in a single query
            var patient = await _context.Patients
                .Include(p => p.Prescriptions)
                    .ThenInclude(pr => pr.Doctor)
                .Include(p => p.Prescriptions)
                    .ThenInclude(pr => pr.PrescriptionMedicaments)
                    .ThenInclude(pm => pm.Medicament)
                .FirstOrDefaultAsync(p => p.IdPatient == id);

            if (patient == null)
            {
                return new GetPatientInfoResponse 
                { 
                    Status = 404, // Use 404 for "not found" instead of 400
                    Message = "Patient not found." 
                };
            }

            // Transform to response DTOs
            var prescriptionInfo = patient.Prescriptions
                .Select(prescription => new PrescriptionInfoResponse
                {
                    Date = prescription.Date,
                    DueDate = prescription.DueDate,
                    Doctor = new DoctorInfoResponse
                    {
                        Id = prescription.Doctor.IdDoctor,
                        Email = prescription.Doctor.Email,
                        FirstName = prescription.Doctor.FirstName,
                        LastName = prescription.Doctor.LastName
                    },
                    Medicaments = prescription.PrescriptionMedicaments
                        .Select(pm => new MedicamentInfoResponse
                        {
                            Id = pm.Medicament.IdMedicament,
                            Description = pm.Medicament.Description,
                            Details = pm.Details,
                            Dose = pm.Dose
                        }).ToList()
                }).ToList();

            return new GetPatientInfoResponse
            {
                Status = 200,
                Message = "Patient found successfully.",
                PatientInfo = new PatientInfoResponse
                {
                    Id = patient.IdPatient,
                    FirstName = patient.FirstName,
                    LastName = patient.LastName,
                    Birthday = patient.Birthday,
                    Prescriptions = prescriptionInfo
                }
            };
        }
        catch (Exception exception)
        {
            Console.WriteLine($"Error getting patient info: {exception.Message}");
            if (exception.InnerException != null)
            {
                Console.WriteLine($"Inner exception: {exception.InnerException.Message}");
            }
            
            return new GetPatientInfoResponse 
            { 
                Status = 500, 
                Message = "Unexpected server error occurred." 
            };
        }
    }
    
}