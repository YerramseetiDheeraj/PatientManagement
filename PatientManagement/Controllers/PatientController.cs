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
        public async Task<IActionResult> AddPatient(Patient patient)
        {
            try
            {
                var id = await _patientRepository.AddPatientAsync(patient);
                return Ok("Patient added successfully");
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Error = ex.Message }); 
            }
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAllPatients()
        {
            var patients = await _patientRepository.GetAllPatientsAsync();
            return Ok(patients);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPatientById([FromRoute] int id)
        {
            try
            {
                var patient = await _patientRepository.GetPatientByIdAsync(id);
                return Ok(patient);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(new { Error = ex.Message }); 
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Error = ex.Message }); 
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePatientById([FromRoute] int id)
        {
            try
            {
                await _patientRepository.DeletePatientByIdAsync(id);
                return Ok($"Patient with Id {id} deleted successfully.");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(new { Error = ex.Message }); 
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Error = ex.Message }); 
            }
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> EditPatientById([FromBody] JsonPatchDocument<Patient> patientModel, [FromRoute] int id)
        {
            try
            {
                await _patientRepository.EditPatientByIdPatchAsync(id, patientModel);
                return Ok($"Patient with Id {id} edited successfully.");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(new { Error = ex.Message }); 
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Error = ex.Message }); 
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Error = ex.Message }); 
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePatientAllDetailsById([FromRoute] int id, [FromBody] Patient patient)
        {
            try
            {
                await _patientRepository.UpdatePatientByIdAsync(id, patient);
                return Ok($"Patient with Id {id} updated successfully.");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(new { Error = ex.Message }); 
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Error = ex.Message }); 
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Error = ex.Message }); 
            }
        }
    }
}
