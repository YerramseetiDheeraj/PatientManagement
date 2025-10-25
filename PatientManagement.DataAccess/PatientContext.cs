using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PatientManagement.Models;

namespace PatientManagement.Data
{
    public class PatientContext : IdentityDbContext<ApplicationAdmin>
    {
        public PatientContext(DbContextOptions<PatientContext> options) : base(options) { }

        public DbSet<Patient> Patients { get; set; }
    }
}
