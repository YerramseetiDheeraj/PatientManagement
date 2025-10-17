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
        public async Task<IActionResult> AddPatientAsync(PatientCreateModel patientModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            bool emailExist = await _patientRepository.IsPatientEmailExistAsync(patientModel.Email);

            if (emailExist)
            {
                return Conflict("Email already Exists");
            }

            try
            {
                await _patientRepository.AddPatientAsync(patientModel);
                return Ok($"Patient added successfully");
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

            if (patients == null)
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

            var patient = await _patientRepository.GetPatientByIdAsync(id);

            if (patient == null)
            {
                return NotFound($"Patient with Id {id} not found");
            }

            return Ok(patient);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePatientByIdAsync([FromRoute] int id)
        {
            if (id <= 0)
            {
                return BadRequest("Id should be greater than zero");
            }

            var patient = await _patientRepository.GetPatientByIdAsync(id);

            if (patient == null)
            {
                return NotFound($"Patient with Id {id} not found");
            }

            await _patientRepository.DeletePatientByIdAsync(id);
            return Ok($"Patient with Id {id} deleted successfully.");

        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> EditPatientByIdAsync([FromBody] JsonPatchDocument<PatientUpdateModel> patientUpdateModel, [FromRoute] int id)
        {
            if (id <= 0)
            {
                return BadRequest("Id should be greater than zero");
            }

            if (patientUpdateModel==null)
            {
                return BadRequest("patient data cannot be null");
            }

            var patient = await _patientRepository.GetPatientByIdAsync(id);

            if (patient == null)
            {
                return NotFound($"Patient with Id {id} not found");
            }

            try
            {
                await _patientRepository.EditPatientByIdPatchAsync(id, patientUpdateModel);
                return Ok($"Patient with Id {id} edited successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePatientAllDetailsByIdAsync([FromRoute] int id, [FromBody] PatientUpdateModel patientUpdateModel)
        {

            if (id <= 0)
            {
                return BadRequest("Id should be greater than zero");
            }

            if(patientUpdateModel == null)
            {
                return BadRequest("Patient data cannot be null");
            }

            var existingPatient = await _patientRepository.GetPatientByIdAsync(id);

            if (existingPatient == null)
            {
                return NotFound($"Patient with Id {id} not found");
            }

            if (existingPatient.Email != patientUpdateModel.Email)
            {
                bool emailExists = await _patientRepository.IsPatientEmailExistAsync(patientUpdateModel.Email);

                if (emailExists)
                {
                    return Conflict("Email already Exists");
                }
            }

            try
            {
                await _patientRepository.UpdatePatientByIdAsync(id, patientUpdateModel);
                return Ok($"Patient with Id {id} updated successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
