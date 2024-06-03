using Microsoft.EntityFrameworkCore;
using Creatore_di_annunci.Models.Entities;

namespace Creatore_di_annunci.Data
{
    public class ApplicationDbContext : DbContext
    {
        //COSTRUTTORE
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        //ATTRIBUTI
        public DbSet<House> houses { get; set; }
        public DbSet<Video> Videos { get; set; }
        public DbSet<Descrizione> Descrizioni { get; set; }
    }
}
