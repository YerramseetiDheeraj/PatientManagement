using Microsoft.AspNetCore.Identity;

namespace PatientManagement.Models
{
    public class ApplicationAdmin : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

    }
}
