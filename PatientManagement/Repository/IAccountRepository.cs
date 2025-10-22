using Microsoft.AspNetCore.Identity;
using PatientManagement.Models;

namespace PatientManagement.Repository
{
    public interface IAccountRepository
    {
        Task<IdentityResult> SignUpAsync(SignUpModel signUpModel);
        Task<string> LoginAsync(SignInModel signInModel);
    }
}
