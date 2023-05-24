using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class DBContextClass: DbContext
    {
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Journal> Journals { get; set; }
        public DbSet<Measurement> Measurements { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=StudiePC;Initial Catalog=EKGPatientDatabase;Integrated Security=True;Encrypt=false");
        }
    }
}
