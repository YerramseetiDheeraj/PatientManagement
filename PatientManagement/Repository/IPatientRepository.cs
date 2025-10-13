using Microsoft.AspNetCore.JsonPatch;
using PatientManagement.Data;
using PatientManagement.Models;

namespace PatientManagement.Repository
{
    public interface IPatientRepository
    {
        Task AddPatientAsync(PatientModel patientModel);

        Task<bool> IsPatientEmailExistAsync(PatientModel patientModel);

        Task<List<Patient>> GetAllPatientsAsync();

        Task<Patient> GetPatientByIdAsync(int id);

        Task DeletePatientByIdAsync(int id);

        Task EditPatientByIdPatchAsync(int id, JsonPatchDocument<PatientModel> patientModel);

        Task UpdatePatientByIdAsync(int id, PatientModel patientModel);
    }
}
