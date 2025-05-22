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
        // Optimistic locking
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

            // Get patient
            Patient patient;
            if (!_context.Patients.Any(e => e.IdPatient == request.Patient.Id))
            {
                patient = new Patient
                {
                    IdPatient = request.Patient.Id,
                    FirstName = request.Patient.FirstName,
                    LastName = request.Patient.LastName,
                    Birthday = request.Patient.BirthDate,
                    Prescriptions = new List<Prescription>()
                };
                _context.Patients.Add(patient);
            }
            else
            {
                patient = _context.Patients.First(e => e.IdPatient == request.Patient.Id);
            }

            // Get the doctor
            Doctor doctor;
            if (_context.Doctors.Any(e => e.IdDoctor == request.DoctorId))
            {
                doctor = _context.Doctors.First(e => e.IdDoctor == request.DoctorId);
            }
            else
            {
                return new NewPrescriptionResponse { Status = 400, Message = "Doctor not found." };
            }

            // Get medicaments
            var medicaments = _context.Medicaments
                .Where(e => request.Medicaments.Any(m => m.Id == e.IdMedicament)).ToList();
            if (medicaments.Count != request.Medicaments.Count)
            {
                return new NewPrescriptionResponse { Status = 400, Message = "Some medicaments not found." };
            }

            // Insert new prescription
            var prescription = new Prescription
            {
                Date = request.Date,
                Doctor = doctor,
                Patient = patient,
                DueDate = request.DueDate,
                PrescriptionMedicaments = new List<PrescriptionMedicament>()
            };
            _context.Prescriptions.Add(prescription);

            // Insert medicament links
            var medicamentsPrescriptions = medicaments.Join(request.Medicaments,
                e => e.IdMedicament, r => r.Id,
                (e, req) => new PrescriptionMedicament
                {
                    Details = req.Details,
                    Dose = req.Dose,
                    Medicament = e,
                    Prescription = prescription
                }
            );
            _context.PrescriptionMedicaments.AddRange(medicamentsPrescriptions);

            // Save changes and return response.
            await _context.SaveChangesAsync();
            return new NewPrescriptionResponse { Status = 200, Message = "Successfully added new prescription. " };
        }
        catch (DbUpdateConcurrencyException exception)
        {
            Console.WriteLine(exception.Message);
            return new NewPrescriptionResponse { Status = 500, Message = "A synchronization error occured. Please try again." };
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception.Message);
            return new NewPrescriptionResponse{ Status = 500, Message = "Unexpected server error took place." };
        }
    }

    public async Task<GetPatientInfoResponse> GetPatientInfo(int id)
    {
        // Optimistic locking
        try
        {
            // Get patient info
            if (!await _context.Patients.AnyAsync(e => e.IdPatient == id))
            {
                return new GetPatientInfoResponse{ Status = 400, Message = "Patient not found." };
            }
            var patient = await _context.Patients.FirstAsync(e => e.IdPatient == id);

            // Get all the prescriptions
            var prescriptions = _context.Prescriptions
                .Include(e => e.PrescriptionMedicaments)
                .ThenInclude(e => e.Medicament)
                .Include(e => e.Doctor)
                .Where(e => e.Patient.IdPatient == id)
                .ToList();

            var prescriptionInfo = prescriptions
                .Select(e => new PrescriptionInfoResponse
                {
                    Date = e.Date,
                    Doctor = new DoctorInfoResponse
                    {
                        Id = e.Doctor.IdDoctor,
                        Email = e.Doctor.Email,
                        FirstName = e.Doctor.FirstName,
                        LastName = e.Doctor.LastName
                    },
                    DueDate = e.DueDate,
                    Medicaments = e.PrescriptionMedicaments.Select(e2 => new MedicamentInfoResponse
                    {
                        Id = e2.Medicament.IdMedicament,
                        Description = e2.Medicament.Description,
                        Details = e2.Details,
                        Dose = e2.Dose
                    }).ToList()
                }).ToList();

            return new GetPatientInfoResponse
            {
                Status = 200,
                Message = "",
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
            Console.WriteLine(exception.Message);
            return new GetPatientInfoResponse{ Status = 500, Message = "Unexpected server error took place." };
        }
    }
    
}