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

        public async Task AddPatientAsync(PatientModel patientModel)
        {
            var patient = _mapper.Map<Patient>(patientModel);

            var today = DateOnly.FromDateTime(DateTime.Now);

            if (patientModel.DateOfBirth < today.AddYears(-150) || patientModel.DateOfBirth > today)
                throw new InvalidOperationException("Invalid Date of Birth. Age must be between 0 and 150 years.");

            if (patientModel.Height <= 0 || patientModel.Height > 10)
                throw new InvalidOperationException("Invalid height. Height must be in ft.inch(5.7) format");

            if (patientModel.Weight <= 0 || patientModel.Weight > 300)
                throw new InvalidOperationException("Invalid Weight. Weight must be in Kg.gm(65.90) format");

            patient.Email = patientModel.Email.ToLower();
            patient.CreateDate = today;
            patient.UpdatedDate = today;

            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsPatientEmailExistAsync(PatientModel patientModel)
        {
            return await _context.Patients.AnyAsync(e => e.Email == patientModel.Email);
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

        public async Task EditPatientByIdPatchAsync(int id, JsonPatchDocument<PatientModel> patientPatch)
        {
            var existingPatient = await GetPatientByIdAsync(id);

            var patientModel = _mapper.Map<PatientModel>(existingPatient);

            patientPatch.ApplyTo(patientModel);

            _mapper.Map(patientModel, existingPatient);

            var today = DateOnly.FromDateTime(DateTime.Now);

            if (existingPatient.DateOfBirth < today.AddYears(-150) || existingPatient.DateOfBirth > today)
                throw new InvalidOperationException("Invalid Date of Birth. Age must be between 0 and 150 years.");

            if (existingPatient.Height <= 0 || existingPatient.Height > 10)
                throw new InvalidOperationException("Invalid height. Height must be in ft.inch(5.7) format");

            if (existingPatient.Weight <= 0 || existingPatient.Weight > 300)
                throw new InvalidOperationException("Invalid Weight. Weight must be in Kg.gm(65.90) format");

            existingPatient.UpdatedDate = DateOnly.FromDateTime(DateTime.Now);

            await _context.SaveChangesAsync();
        }

        public async Task UpdatePatientByIdAsync(int id, PatientModel patientModel)
        {
            var existingPatient = await GetPatientByIdAsync(id);

            var patient = _mapper.Map(patientModel, existingPatient);

            var today = DateOnly.FromDateTime(DateTime.Now);

            if (patient.DateOfBirth < today.AddYears(-150) || patient.DateOfBirth > today)
                throw new InvalidOperationException("Invalid Date of Birth. Age must be between 0 and 150 years.");

            if (patient.Height <= 0 || patient.Height > 10)
                throw new InvalidOperationException("Invalid height. Height must be in ft.inch(5.7) format");

            if (patient.Weight <= 0 || patient.Weight > 300)
                throw new InvalidOperationException("Invalid Weight. Weight must be in Kg.gm(65.90) format");

            patient.Email = patient.Email.ToLower();
            patient.CreateDate = existingPatient.CreateDate;
            patient.UpdatedDate = DateOnly.FromDateTime(DateTime.Now);

            await _context.SaveChangesAsync();
        }
    }
}
