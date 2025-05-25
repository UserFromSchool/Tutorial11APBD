using Microsoft.EntityFrameworkCore;
using Tutorial11.Models;

namespace Tutorial11.Data;

public class DatabaseContext(IConfiguration configuration) : DbContext
{
    private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Medicament> Medicaments { get; set; }
    public DbSet<PrescriptionMedicament> PrescriptionMedicaments { get; set; }
    public DbSet<Prescription> Prescriptions { get; set; }
    public DbSet<Patient> Patients { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Doctor>().HasData(
            new Doctor
            {
                IdDoctor = 1,
                FirstName = "John",
                LastName = "Smith",
                Email = "john.smith@hospital.com"
            },
            new Doctor
            {
                IdDoctor = 2,
                FirstName = "Sarah",
                LastName = "Johnson",
                Email = "sarah.johnson@hospital.com"
            }
        );

        // Seed Patient
        modelBuilder.Entity<Patient>().HasData(
            new Patient
            {
                IdPatient = 1,
                FirstName = "Michael",
                LastName = "Brown",
                Birthday = new DateTime(1985, 3, 15)
            }
        );

        // Seed Medicaments
        modelBuilder.Entity<Medicament>().HasData(
            new Medicament
            {
                IdMedicament = 1,
                Name = "Ibuprofen",
                Description = "Anti-inflammatory pain reliever",
                Type = "NSAID"
            },
            new Medicament
            {
                IdMedicament = 2,
                Name = "Amoxicillin",
                Description = "Antibiotic for bacterial infections",
                Type = "Antibiotic"
            },
            new Medicament
            {
                IdMedicament = 3,
                Name = "Lisinopril",
                Description = "ACE inhibitor for blood pressure",
                Type = "ACE Inhibitor"
            },
            new Medicament
            {
                IdMedicament = 4,
                Name = "Metformin",
                Description = "Diabetes medication",
                Type = "Antidiabetic"
            },
            new Medicament
            {
                IdMedicament = 5,
                Name = "Omeprazole",
                Description = "Proton pump inhibitor for acid reflux",
                Type = "PPI"
            },
            new Medicament
            {
                IdMedicament = 6,
                Name = "Atorvastatin",
                Description = "Cholesterol lowering medication",
                Type = "Statin"
            },
            new Medicament
            {
                IdMedicament = 7,
                Name = "Lorazepam",
                Description = "Anti-anxiety medication",
                Type = "Benzodiazepine"
            }
        );

        // Seed Prescriptions
        modelBuilder.Entity<Prescription>().HasData(
            new Prescription
            {
                IdPrescrription = 1,
                Date = new DateTime(2024, 5, 15),
                DueDate = new DateTime(2024, 6, 14),
                IdDoctor = 1,
                IdPatient = 1
            },
            new Prescription
            {
                IdPrescrription = 2,
                Date = new DateTime(2024, 5, 20),
                DueDate = new DateTime(2024, 6, 19),
                IdDoctor = 2,
                IdPatient = 1
            },
            new Prescription
            {
                IdPrescrription = 3,
                Date = new DateTime(2024, 5, 23),
                DueDate = new DateTime(2024, 6, 22),
                IdDoctor = 1,
                IdPatient = 1
            }
        );

        // Seed PrescriptionMedicaments (Many-to-Many relationships)
        modelBuilder.Entity<PrescriptionMedicament>().HasData(
            // Prescription 1 (Doctor 1) - 3 medicaments
            new PrescriptionMedicament
            {
                IdPrescription = 1,
                IdMedicament = 1, // Ibuprofen
                Dose = 400,
                Details = "Take twice daily with food"
            },
            new PrescriptionMedicament
            {
                IdPrescription = 1,
                IdMedicament = 2, // Amoxicillin
                Dose = 500,
                Details = "Take three times daily for 7 days"
            },
            new PrescriptionMedicament
            {
                IdPrescription = 1,
                IdMedicament = 3, // Lisinopril
                Dose = 10,
                Details = "Take once daily in the morning"
            },

            // Prescription 2 (Doctor 2) - 2 medicaments
            new PrescriptionMedicament
            {
                IdPrescription = 2,
                IdMedicament = 4, // Metformin
                Dose = 850,
                Details = "Take with breakfast and dinner"
            },
            new PrescriptionMedicament
            {
                IdPrescription = 2,
                IdMedicament = 5, // Omeprazole
                Dose = 20,
                Details = "Take 30 minutes before breakfast"
            },

            // Prescription 3 (Doctor 1) - 2 medicaments
            new PrescriptionMedicament
            {
                IdPrescription = 3,
                IdMedicament = 6, // Atorvastatin
                Dose = 20,
                Details = "Take once daily at bedtime"
            },
            new PrescriptionMedicament
            {
                IdPrescription = 3,
                IdMedicament = 7, // Lorazepam
                Dose = 1,
                Details = "Take as needed for anxiety, max 3 times daily"
            }
        );

        base.OnModelCreating(modelBuilder);
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(_connectionString);
    }
    
}