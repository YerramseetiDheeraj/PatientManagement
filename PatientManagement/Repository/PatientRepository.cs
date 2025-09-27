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

        public async Task<string> AddPatientAsync(Patient patient)
        {
            if (patient == null)
                throw new ArgumentNullException(nameof(patient), "Employee cannot be null");

            bool exists = await _context.Patients.AnyAsync(e => e.Email == patient.Email);
            if (exists)
                throw new InvalidOperationException("Patient with this email already exists");

            patient.CreateDate = DateOnly.FromDateTime(DateTime.Now);
            patient.UpdatedDate = DateOnly.FromDateTime(DateTime.Now);

            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();
            return $"Patient added Succesfully";
        }

        public async Task<List<Patient>> GetAllPatientsAsync()
        {
            return await _context.Patients.ToListAsync();
        }

        public async Task<Patient> GetPatientByIdAsync(int id)
        {
            return await _context.Patients.FindAsync(id);
        }

        public async Task<string> DeletePatientByIdAsync(int id)
        {
            var patients = await _context.Patients.FindAsync(id);

            if (patients != null)
            {
                 _context.Patients.Remove(patients);
                await _context.SaveChangesAsync();
                return $"Employee With Id{id} Deleted Succesfully";
            }

            return $"Error Occured Employee With Id{id} Not Deleted ";
        }

        public async Task<string> EditPatientByIdPatchAsync(int id, JsonPatchDocument<Patient> patientModel)
        {
            var patients = await _context.Patients.FindAsync(id);
            if (patients != null)
            {
                patientModel.ApplyTo(patients);
                await _context.SaveChangesAsync();
                return $"Patient With Id {id} Details Changed Succesfully";
            }

            return $" Error Occured Patient With Id {id} Details not changed";
        }

        public async Task<string> EditPatientAllDetailsByIdAsync(int id, Patient patient)
        {
            var existingPatient = await _context.Patients.FindAsync(id);

            if(existingPatient == null)
            {
                return $"Patiend with Id {id} Does Not Exist";
            }

            patient.CreateDate = existingPatient.CreateDate;
            patient.UpdatedDate = DateOnly.FromDateTime(DateTime.Now);
            patient.Id = id;

            _context.Entry(existingPatient).CurrentValues.SetValues(patient);

            await _context.SaveChangesAsync();
            return $"Patient With Id {id} Details Changed Succesfully";
        }
    }
}
