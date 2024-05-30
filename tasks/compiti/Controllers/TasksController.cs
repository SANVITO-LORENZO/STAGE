using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using compiti.Data;
using compiti.Models.DTO;
using compiti.Models.Entities;
using compiti.Repositories.Interfaces;
using compiti.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;
namespace compiti.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompitoController : ControllerBase
    {
        private readonly ITaskRepository taskRepository;
        private readonly ApplicationDbContext dbContext;

        public CompitoController(ITaskRepository taskRepository, ApplicationDbContext dbContext)
        {
            this.taskRepository = taskRepository;
            this.dbContext = dbContext;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateTask(CreateTaskRequestDto request)
        {
            var task = new compiti.Models.Entities.Task
            {
                Id = request.Id,
                Name = request.Name,
                Subject = request.Subject,
                DateTime = request.DateTime,
                Description = request.Description,
                IsCompleted = request.IsCompleted

            };

            await taskRepository.CreateAsync(task);

            var response = new TaskDto
            {
                Id = task.Id,
                Name = request.Name,
                Description = request.Description,
                DateTime = request.DateTime,
                IsCompleted = request.IsCompleted,
                Subject = request.Subject
            };
            return Ok(response);
        }

        [HttpPost("Edit")]
        public async Task<IActionResult> EditTask(EditTaskRequestDto request)
        {
            var task = await dbContext.Tasks.FindAsync(request.Id);

            if (task == null)
            {
                return NotFound();
            }

            task.Name = request.Name;
            task.Description = request.Description;
            task.IsCompleted = request.IsCompleted;
            task.Subject = request.Subject;
            task.DateTime = request.DateTime;

            await dbContext.SaveChangesAsync();

            var response = new TaskDto
            {
                Id = task.Id,
                Name = task.Name,
                Description = task.Description,
                DateTime = task.DateTime,
                IsCompleted = task.IsCompleted,
                Subject = task.Subject
            };

            return Ok(response);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteTask(Guid id)
        {
            var task = await dbContext.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            dbContext.Tasks.Remove(task);
            await dbContext.SaveChangesAsync();

            return Ok();
        }
    }
}