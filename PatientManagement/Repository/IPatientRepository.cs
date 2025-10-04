using Microsoft.AspNetCore.JsonPatch;
using PatientManagement.Models;

namespace PatientManagement.Repository
{
    public interface IPatientRepository
    {
        Task<int> AddPatientAsync(Patient patient);

        Task<List<Patient>> GetAllPatientsAsync();

        Task<Patient> GetPatientByIdAsync(int id);

        Task DeletePatientByIdAsync(int id);

        Task EditPatientByIdPatchAsync(int id, JsonPatchDocument<Patient> patientModel);

        Task UpdatePatientByIdAsync(int id, Patient patient);
    }
}
