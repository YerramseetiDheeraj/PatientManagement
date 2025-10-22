using LazyCache;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PatientManagement.Models;
using PatientManagement.Repository;

namespace PatientManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : Controller
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ICacheProvider _cacheProvider;

        public AccountController(IAccountRepository accountRepository, ICacheProvider cacheProvider)
        {
            _accountRepository = accountRepository;
            _cacheProvider = cacheProvider;
        }

        [AllowAnonymous]
        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] SignUpModel signUpModel)
        {
            var result = await _accountRepository.SignUpAsync(signUpModel);

            if (result.Succeeded)
                return Ok("User created successfully");

            return BadRequest(result.Errors.Select(e => e.Description));
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] SignInModel signInModel)
        {
            var token = await _accountRepository.LoginAsync(signInModel);

            if (string.IsNullOrEmpty(token))
                return Unauthorized("Invalid login attempt");

            return Ok(token);
        }
    }
}
