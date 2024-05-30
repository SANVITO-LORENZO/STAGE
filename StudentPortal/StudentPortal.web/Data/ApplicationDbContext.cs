using Microsoft.EntityFrameworkCore;
using StudentPortal.web.Models.Entities;

namespace StudentPortal.web.Data
{
    public class ApplicationDbContext : DbContext
    {
        //COSTRUTTORE
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        //ATTRIBUTI
        public DbSet<Student> students { get; set; }
    }
}
