using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using PatientManagement.Data;
using PatientManagement.Models;
using System.Reflection;
using System.Text;
using System.Linq.Dynamic.Core;

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

        public async Task<List<Patient>> GetAllPatientsAsync(string ?term,string ?sort,int page,int limit)
        {
            IQueryable<Patient> patients;

            //searching
            if (string.IsNullOrEmpty(term))
            {
                patients = _context.Patients;
            }
            else
            {
                term = term.Trim().ToLower();

                patients = _context.Patients.Where(p => p.FirstName.ToLower().Contains(term)||
                p.LastName.ToLower().Contains(term)||
                p.Address.ToLower().Contains(term)||
                p.Gender.ToLower().Contains(term)||
                p.MedicalComments.ToLower().Contains(term)||
                p.Email.ToLower().Contains(term)||
                p.ContactNumber.ToLower().Contains(term));
            }

            //sorting
            if (!string.IsNullOrWhiteSpace(sort))
            {
                var sortFields = sort.Split(',');
                StringBuilder orderQueryBuilder = new StringBuilder();
                PropertyInfo[] propertyInfo = typeof(Patient).GetProperties();

                foreach (var field in sortFields)
                {
                    string sortOrder = "ascending";
                    var sortField = field.Trim();

                    if (sortField.StartsWith("-"))
                    {
                        sortField = sortField.TrimStart('-');
                        sortOrder = "descending";
                    }

                    var property = propertyInfo.FirstOrDefault(x => x.Name.Equals(sortField, StringComparison.OrdinalIgnoreCase));

                    if (property == null)
                        continue;

                    orderQueryBuilder.Append($"{property.Name.ToString()}{sortOrder},");
                }

                string orderQuery = orderQueryBuilder.ToString().TrimEnd(',', ' ');

                if (!string.IsNullOrWhiteSpace(orderQuery))
                {
                    patients = patients.OrderBy(orderQuery);
                }
                else
                {
                    patients = patients.OrderBy(a => a.Id);
                }
            }

            //applying pagination
            var totalCount = await _context.Patients.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)limit);
            var paged = await patients.Skip((page - 1) * limit).Take(limit).ToListAsync();
            return paged;
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
