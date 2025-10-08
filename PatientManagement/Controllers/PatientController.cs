using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using PatientManagement.Models;
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
        public async Task<IActionResult> AddPatientAsync(Patient patient)
        {
            if (patient == null)
            {
                return BadRequest("Patient cannot be null");
            }

            try
            {
                var id = await _patientRepository.AddPatientAsync(patient);
                return Ok("Patient added successfully");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAllPatientsAsync()
        {
            var patients = await _patientRepository.GetAllPatientsAsync();

            if(patients == null)
            {
                return NotFound();
            }

            return Ok(patients);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPatientByIdAsync([FromRoute] int id)
        {
            if (id <= 0)
            {
                return BadRequest("Id should be greater than zero");
            }

            try
            {
                var patient = await _patientRepository.GetPatientByIdAsync(id);
                return Ok(patient);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePatientByIdAsync([FromRoute] int id)
        {
            if (id <= 0)
            {
                return BadRequest("Id should be greater than zero");
            }

            try
            {
                await _patientRepository.DeletePatientByIdAsync(id);
                return Ok($"Patient with Id {id} deleted successfully.");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> EditPatientByIdAsync([FromBody] JsonPatchDocument<Patient> patientModel, [FromRoute] int id)
        {

            if (id <= 0)
            {
                return BadRequest("Id should be greater than zero");
            }

            if(patientModel == null)
            {
                return BadRequest("Patient cannot be null");
            }

            try
            {
                await _patientRepository.EditPatientByIdPatchAsync(id, patientModel);
                return Ok($"Patient with Id {id} edited successfully.");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePatientAllDetailsByIdAsync([FromRoute] int id, [FromBody] Patient patient)
        {

            if (id <= 0)
            {
                return BadRequest("Id should be greater than zero");
            }

            if (patient == null)
            {
                return BadRequest("Patient cannot be null");
            }

            try
            {
                await _patientRepository.UpdatePatientByIdAsync(id, patient);
                return Ok($"Patient with Id {id} updated successfully.");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
