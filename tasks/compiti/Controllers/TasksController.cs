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

        [HttpPost]
        public async Task<IActionResult> CreateTask(CreateTaskRequestDto request)
        {
            var task = new compiti.Models.Entities.Task
            {
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
            return Ok(task);
        }

        [HttpPost]
        public async Task<IActionResult> EditTask(compiti.Models.Entities.Task viewModel)
        {
            var task = await dbContext.Tasks.FindAsync(viewModel.Id);

            if (task is not null)
            {
                task.Name = viewModel.Name;
                task.Description = viewModel.Description;
                task.IsCompleted = viewModel.IsCompleted;
                task.Subject = viewModel.Subject;
                task.DateTime = viewModel.DateTime;

                await dbContext.SaveChangesAsync();
            }


            return Ok(task);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTask(compiti.Models.Entities.Task _task)
        {
            var task = await dbContext.Tasks.FindAsync(_task.Id);
            if (task is not null)
            {
                dbContext.Tasks.Remove(task);
                await dbContext.SaveChangesAsync();
            }

            return Ok(task);
        }


    }
}
