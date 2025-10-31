using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using PatientManagement.Caching;
using PatientManagement.Data;
using PatientManagement.Models;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Text;

namespace PatientManagement.Repository
{
    public class PatientRepository : IPatientRepository
    {
        private readonly PatientContext _context;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cacheProvider;

        public PatientRepository(PatientContext context, IMapper mapper, IMemoryCache memoryCache)
        {
            _context = context;
            _mapper = mapper;
            _cacheProvider = memoryCache;
        }

        private void ClearPatientCache()
        {
            if(_cacheProvider is MemoryCache memoryCache)
            {
                memoryCache.Compact(1.0);
            }
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
            ClearPatientCache();
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsPatientEmailExistAsync(string email)
        {
            return await _context.Patients.AnyAsync(e => e.Email == email);
        }

        public async Task<List<Patient>> GetAllPatientsAsync(string? term, string? sort, int page, int limit)
        {
            string cacheKeys = $"{CacheKeys.Patient}_{term}_{sort}_{page}_{limit}";

            if (!_cacheProvider.TryGetValue(cacheKeys, out List<Patient> patients))
            {
                IQueryable<Patient> query;

                //searching
                if (string.IsNullOrEmpty(term))
                {
                    query = _context.Patients;
                }
                else
                {
                    term = term.Trim().ToLower();

                    query = _context.Patients.Where(p => p.FirstName.ToLower().Contains(term) ||
                    p.LastName.ToLower().Contains(term) ||
                    p.Address.ToLower().Contains(term) ||
                    p.Gender.ToLower().Contains(term) ||
                    p.MedicalComments.ToLower().Contains(term) ||
                    p.Email.ToLower().Contains(term) ||
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
                        query = query.OrderBy(orderQuery);
                    }
                    else
                    {
                        query = query.OrderBy(a => a.Id);
                    }
                }

                //applying pagination
                var totalCount = await _context.Patients.CountAsync();
                var totalPages = (int)Math.Ceiling(totalCount / (double)limit);
                patients = await query.Skip((page - 1) * limit).Take(limit).ToListAsync();

                if (patients != null)
                {
                    var cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Now.AddHours(7),
                        SlidingExpiration = TimeSpan.FromHours(7)
                    };
                    _cacheProvider.Set(cacheKeys, patients, cacheEntryOptions);
                }

            }

            return patients;
        }

        public async Task<Patient> GetPatientByIdAsync(int id)
        {
            var cacheKeys = $"{CacheKeys.Patient}_{id}";

            if (!_cacheProvider.TryGetValue(cacheKeys, out Patient patients))
            {
                 patients = await _context.Patients.FindAsync(id);

                if(patients != null)
                {
                    var cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Now.AddHours(7),
                        SlidingExpiration = TimeSpan.FromHours(7)
                    };
                    _cacheProvider.Set(cacheKeys, patients, cacheEntryOptions);
                }

            }

            return patients;
        }

        public async Task DeletePatientByIdAsync(int id)
        {
            var patient = await GetPatientByIdAsync(id);

            _context.Patients.Remove(patient);
            ClearPatientCache();
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
            ClearPatientCache();
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
            ClearPatientCache();
            await _context.SaveChangesAsync();
        }
    }
}
