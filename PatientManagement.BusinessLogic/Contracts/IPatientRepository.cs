using Microsoft.AspNetCore.JsonPatch;
using PatientManagement.Data;
using PatientManagement.Models;

namespace PatientManagement.Repository
{
    public interface IPatientRepository
    {
        Task AddPatientAsync(PatientCreateModel patientCreateModel);

        Task<bool> IsPatientEmailExistAsync(string email);

        Task<List<Patient>> GetAllPatientsAsync(string? term, string? sort, int page, int limit);

        Task<Patient> GetPatientByIdAsync(int id);

        Task DeletePatientByIdAsync(int id);

        Task EditPatientByIdPatchAsync(int id, JsonPatchDocument<PatientUpdateModel> patientUpdateModel);

        Task UpdatePatientByIdAsync(int id, PatientUpdateModel patientUpdateModel);
    }
}
