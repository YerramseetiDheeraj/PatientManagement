using PatientManagement.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using PatientManagement.Data;
using System.Net;

namespace PatientManagement.Repository
{
    public class PatientRepository : IPatientRepository
    {
        private readonly PatientContext _context;

        public PatientRepository(PatientContext context)
        {
            _context = context;
        }

        public async Task<int> AddPatientAsync(Patient patient)
        {
            if (patient == null)
                throw new ArgumentNullException("Employee cannot be null");

            if (await _context.Patients.AnyAsync(e => e.Email == patient.Email))
                throw new InvalidOperationException("Patient with this email already exists");

            var today = DateOnly.FromDateTime(DateTime.Now);

            if (patient.DateOfBirth < today.AddYears(-150) || patient.DateOfBirth > today)
                throw new InvalidOperationException("Invalid Date of Birth. Age must be between 0 and 150 years.");

            if (patient.Height <=0 || patient.Height>10)
                throw new InvalidOperationException("Invalid height. Height must be in ft.inch(5.7) format");

            if(patient.Weight<=0 || patient.Weight>300)
                throw new InvalidOperationException("Invalid Weight. Weight must be in Kg.gm(65.90) format");

            patient.CreateDate = today;
            patient.UpdatedDate = today;

            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();
            return patient.Id;
        }

        public async Task<List<Patient>> GetAllPatientsAsync()
        {
            return await _context.Patients.ToListAsync();
        }

        public async Task<Patient> GetPatientByIdAsync(int id)
        {
            var patient = await _context.Patients.FindAsync(id);

            if (id <= 0)
                throw new ArgumentOutOfRangeException("id should be greater than zero");

            if (patient == null)
                throw new KeyNotFoundException($"Patient with Id {id} does not exist.");

            return patient;
        }

        public async Task DeletePatientByIdAsync(int id)
        {
            var patient = await _context.Patients.FindAsync(id);

            if (id <= 0)
                throw new ArgumentOutOfRangeException("id should be greater than zero");

            if (patient == null)
                throw new KeyNotFoundException($"Patient with Id {id} does not exist.");

            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();
        }

        public async Task EditPatientByIdPatchAsync(int id, JsonPatchDocument<Patient> patientModel)
        {
            var patient = await _context.Patients.FindAsync(id);

            if (id <= 0)
                throw new ArgumentOutOfRangeException("id should be greater than zero");

            if (patient == null)
                throw new KeyNotFoundException($"Patient with Id {id} does not exist.");

            var today = DateOnly.FromDateTime(DateTime.Now);

            if (patient.DateOfBirth < today.AddYears(-150) || patient.DateOfBirth > today)
                throw new InvalidOperationException("Invalid Date of Birth. Age must be between 0 and 150 years.");

            if (patient.Height <= 0 || patient.Height > 10)
                throw new InvalidOperationException("Invalid height. Height must be in ft.inch(5.7) format");

            if (patient.Weight <= 0 || patient.Weight > 300)
                throw new InvalidOperationException("Invalid Weight. Weight must be in Kg.gm(65.90) format");

            patientModel.ApplyTo(patient);
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePatientByIdAsync(int id, Patient patient)
        {
            var existingPatient = await _context.Patients.FindAsync(id);

            if (id <= 0)
                throw new ArgumentOutOfRangeException("id should be greater than zero");

            if (existingPatient == null)
                throw new KeyNotFoundException($"Patient with Id {id} does not exist.");

            var today = DateOnly.FromDateTime(DateTime.Now);

            if (patient.DateOfBirth < today.AddYears(-150) || patient.DateOfBirth > today)
                throw new InvalidOperationException("Invalid Date of Birth. Age must be between 0 and 150 years.");

            if (patient.Height <= 0 || patient.Height > 10)
                throw new InvalidOperationException("Invalid height. Height must be in ft.inch(5.7) format");

            if (patient.Weight <= 0 || patient.Weight > 300)
                throw new InvalidOperationException("Invalid Weight. Weight must be in Kg.gm(65.90) format");

            patient.CreateDate = existingPatient.CreateDate;
            patient.UpdatedDate = DateOnly.FromDateTime(DateTime.Now);
            patient.Id = id;

            _context.Entry(existingPatient).CurrentValues.SetValues(patient);

            await _context.SaveChangesAsync();
        }
    }
}
