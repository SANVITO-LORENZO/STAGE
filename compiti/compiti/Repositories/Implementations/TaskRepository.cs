using compiti.Data;
using compiti.Repositories.Interfaces;

namespace compiti.Repositories.Implementations
{
    public class TaskRepository : ITaskRepository
    {
        private readonly ApplicationDbContext context;

        public TaskRepository(ApplicationDbContext dbcontext)
        {
            this.context = dbcontext;
        }
        public async Task<compiti.Models.Entities.Task> CreateAsync(compiti.Models.Entities.Task task)
        {
            await context.Tasks.AddAsync(task);
            await context.SaveChangesAsync();
            return task;
        }

    }
}
