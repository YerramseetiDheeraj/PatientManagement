using PatientManagement.Models;
using Microsoft.AspNetCore.JsonPatch;

namespace PatientManagement.Repository
{
    public interface IPatientRepository
    {
        Task<string> AddPatientAsync(Patient patient);

        Task<List<Patient>> GetAllPatientsAsync();

        Task<Patient> GetPatientByIdAsync(int id);

        Task<string> DeletePatientByIdAsync(int id);

        Task<string> EditPatientByIdPatchAsync(int id, JsonPatchDocument<Patient> patientModel);

        Task<string> UpdatePatientByIdAsync(int id, Patient patient);
    }
}
