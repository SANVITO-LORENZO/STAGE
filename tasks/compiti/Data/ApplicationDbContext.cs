using Microsoft.EntityFrameworkCore;
using compiti.Models.Entities;

namespace compiti.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<Models.Entities.Task> Tasks { get; set; }
    }
}
