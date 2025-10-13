using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PatientManagement.Data
{
    public class Patient
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        [StringLength(50, ErrorMessage = "First Name cannot be longer than 50 characters")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        [StringLength(50, ErrorMessage = "Last Name cannot be longer than 50 characters")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Date of Birth is required")]
        [Display(Name = "Date of Birth in (DD/MM/YYYY) format")]
        public DateOnly DateOfBirth { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        [StringLength(10)]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Contact Number is required")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        [StringLength(15)]
        public string ContactNumber { get; set; }

        [Required(ErrorMessage = "Weight is required")]
        [Column(TypeName = "decimal(5,2)")]
        [Display(Name = "Weight in kg")]
        public decimal Weight { get; set; }

        [Required(ErrorMessage = "Height is required")]
        [Column(TypeName = "decimal(4,2)")]
        [Display(Name = "Height in ft")]
        public decimal Height { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [StringLength(100)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [StringLength(250)]
        public string Address { get; set; }

        [StringLength(500)]
        public string MedicalComments { get; set; }

        [Required(ErrorMessage = "Medical History Info is required")]
        public bool AnyMedicalTakings { get; set; }

        [Required]
        public DateOnly CreateDate { get; set; }

        [Required]
        public DateOnly UpdatedDate { get; set; }
    }
}
