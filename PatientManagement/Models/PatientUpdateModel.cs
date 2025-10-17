using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PatientManagement.Models
{
    public class PatientUpdateModel
    {
        [StringLength(50, ErrorMessage = "First Name cannot be longer than 50 characters")]
        public string FirstName { get; set; }

        [StringLength(50, ErrorMessage = "Last Name cannot be longer than 50 characters")]
        public string LastName { get; set; }

        [Display(Name = "Date of Birth in (DD/MM/YYYY) format")]
        public DateOnly DateOfBirth { get; set; }

        [StringLength(10)]
        public string Gender { get; set; }

        [Phone(ErrorMessage = "Invalid phone number format")]
        [StringLength(15)]
        public string ContactNumber { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        [Display(Name = "Weight in kg")]
        public decimal Weight { get; set; }

        [Column(TypeName = "decimal(4,2)")]
        [Display(Name = "Height in ft")]
        public decimal Height { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address")]
        [StringLength(100)]
        public string Email { get; set; }

        [StringLength(250)]
        public string Address { get; set; }

        [StringLength(500)]
        public string MedicalComments { get; set; }

        public bool AnyMedicalTakings { get; set; }
    }
}
