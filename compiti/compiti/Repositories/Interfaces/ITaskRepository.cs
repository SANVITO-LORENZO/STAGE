using compiti.Models.Entities;
namespace compiti.Repositories.Interfaces
{
    public interface ITaskRepository
    {
        Task<compiti.Models.Entities.Task> CreateAsync(compiti.Models.Entities.Task task);

    }
}
