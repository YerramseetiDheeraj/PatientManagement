using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using PatientManagement.Data;
using PatientManagement.Models;

namespace PatientManagement.Repository
{
    public class PatientRepository : IPatientRepository
    {
        private readonly PatientContext _context;
        private readonly IMapper _mapper;

        public PatientRepository(PatientContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task AddPatientAsync(PatientCreateModel patientCreateModel)
        {
            var patient = _mapper.Map<Patient>(patientCreateModel);

            var today = DateOnly.FromDateTime(DateTime.Now);

            if (patientCreateModel.DateOfBirth < today.AddYears(-150) || patientCreateModel.DateOfBirth > today)
                throw new InvalidOperationException("Invalid Date of Birth. Age must be between 0 and 150 years.");

            if (patientCreateModel.Height <= 0 || patientCreateModel.Height > 10)
                throw new InvalidOperationException("Invalid height. Height must be in ft.inch(5.7) format");

            if (patientCreateModel.Weight <= 0 || patientCreateModel.Weight > 300)
                throw new InvalidOperationException("Invalid Weight. Weight must be in Kg.gm(65.90) format");

            patient.Email = patientCreateModel.Email.ToLower();

            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsPatientEmailExistAsync(string email)
        {
            return await _context.Patients.AnyAsync(e => e.Email == email);
        }

        public async Task<List<Patient>> GetAllPatientsAsync()
        {
            return await _context.Patients.ToListAsync();
        }

        public async Task<Patient> GetPatientByIdAsync(int id)
        {
            var patient = await _context.Patients.FindAsync(id);

            return patient;
        }

        public async Task DeletePatientByIdAsync(int id)
        {
            var patient = await GetPatientByIdAsync(id);

            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();
        }

        public async Task EditPatientByIdPatchAsync(int id, JsonPatchDocument<PatientUpdateModel> patientPatch)
        {

            var existingPatient = await GetPatientByIdAsync(id);

            var patientModel = _mapper.Map<PatientUpdateModel>(existingPatient);

            patientPatch.ApplyTo(patientModel);

            _mapper.Map(patientModel, existingPatient);

            var today = DateOnly.FromDateTime(DateTime.Now);

            if (existingPatient.DateOfBirth < today.AddYears(-150) || existingPatient.DateOfBirth > today)
                throw new InvalidOperationException("Invalid Date of Birth. Age must be between 0 and 150 years.");

            if (existingPatient.Height <= 0 || existingPatient.Height > 10)
                throw new InvalidOperationException("Invalid height. Height must be in ft.inch(5.7) format");

            if (existingPatient.Weight <= 0 || existingPatient.Weight > 300)
                throw new InvalidOperationException("Invalid Weight. Weight must be in Kg.gm(65.90) format");

            existingPatient.Email = existingPatient.Email.ToLower();

            await _context.SaveChangesAsync();
        }

        public async Task UpdatePatientByIdAsync(int id, PatientUpdateModel patientUpdateModel)
        {
            var existingPatient = await GetPatientByIdAsync(id);

            _mapper.Map(patientUpdateModel, existingPatient);

            var today = DateOnly.FromDateTime(DateTime.Now);

            if (existingPatient.DateOfBirth < today.AddYears(-150) || existingPatient.DateOfBirth > today)
                throw new InvalidOperationException("Invalid Date of Birth. Age must be between 0 and 150 years.");

            if (existingPatient.Height <= 0 || existingPatient.Height > 10)
                throw new InvalidOperationException("Invalid height. Height must be in ft.inch(5.7) format");

            if (existingPatient.Weight <= 0 || existingPatient.Weight > 300)
                throw new InvalidOperationException("Invalid Weight. Weight must be in Kg.gm(65.90) format");

            existingPatient.Email = existingPatient.Email.ToLower();

            await _context.SaveChangesAsync();
        }
    }
}
