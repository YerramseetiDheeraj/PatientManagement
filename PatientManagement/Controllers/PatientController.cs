using PatientManagement.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using PatientManagement.Repository;

namespace PatientManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : Controller
    {
        private readonly IPatientRepository _patientRepository;

        public PatientController(IPatientRepository patientRepository)
        {
            _patientRepository = patientRepository;
        }

        [HttpPost("")]
        public async Task<IActionResult>AddPatient(Patient patient)
        {
            var patients = await _patientRepository.AddPatientAsync(patient);
            return Ok(patients);
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAllPatients()
        {
            var patients= await _patientRepository.GetAllPatientsAsync();
            return Ok(patients);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult>GetPatientById([FromRoute]int id)
        {
            var patients= await _patientRepository.GetPatientByIdAsync(id);
            return Ok(patients);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePatientById([FromRoute]int id)
        {
            await _patientRepository.DeletePatientByIdAsync(id);
            return Ok();
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> EditPatientById([FromBody] JsonPatchDocument<Patient> patientModel, [FromRoute]int id)
        {
            await _patientRepository.EditPatientByIdPatchAsync(id,patientModel);
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditPatientAllDetailsById([FromRoute]int id, [FromBody]Patient patient)
        {
            await _patientRepository.UpdatePatientByIdAsync(id,patient);
            return Ok();
        }
    }
}
