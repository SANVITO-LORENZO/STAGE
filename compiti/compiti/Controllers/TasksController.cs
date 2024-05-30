//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using compiti.Data;
//using compiti.Models;
//using compiti.Models.Entities;

//namespace compiti.Controllers
//{        
//    //[ApiController]
//    //[Route("[controller]")]
//    public class TasksController : Controller
//    {

//        //ATTRIBUTO
//        private readonly ApplicationDbContext dbContext;
//        //COSTRUTTORE
//        public TasksController(ApplicationDbContext dbContext)
//        {
//            this.dbContext = dbContext;
//        }
//        //METODI
//        public IActionResult Index()
//        {
//            return View();
//        }

//        [HttpGet]
//        public IActionResult AddTask()
//        {
//            return View();
//        }
//        [HttpPost]
//        public async Task<IActionResult> AddTask(AddTasksViewModel viewModel)
//        {
//            var task = new Models.Entities.Task
//            {
//                Name = viewModel.Name,
//                Description = viewModel.Description,
//                Subject=viewModel.subject,
//                IsCompleted=viewModel.IsCompleted,
//                DateTime=viewModel.DateTime
//            };
//            await dbContext.Tasks.AddAsync(task);
//            await dbContext.SaveChangesAsync();
//            return View();
//        }
//        [HttpGet]
//        public async Task<IActionResult> List()
//        {
//            var tasks = await dbContext.Tasks.ToListAsync();
//            return View(tasks);
//        }
//        [HttpGet]
//        public async Task<IActionResult> EditTask(Guid id)
//        {
//            var tasks = await dbContext.Tasks.FindAsync(id);
//            return View(tasks);
//        }
//        [HttpPost]
//        public async Task<IActionResult> EditTask(Models.Entities.Task viewModel)
//        {
//            var task = await dbContext.Tasks.FindAsync(viewModel.Id);

//            if (task is not null)
//            {
//                task.Name = viewModel.Name;
//                task.Description = viewModel.Description;
//                task.Subject = viewModel.Subject;
//                task.IsCompleted = viewModel.IsCompleted;
//                task.DateTime = viewModel.DateTime;

//                await dbContext.SaveChangesAsync();
//            }

//            return RedirectToAction("List", "Tasks");
//        }
//        [HttpPost]
//        public async Task<IActionResult> DeleteTask(Models.Entities.Task viewModel)
//        {
//            var task = await dbContext.Tasks.FindAsync(viewModel.Id);
//            if (task is not null)
//            {
//                dbContext.Tasks.Remove(task);
//                await dbContext.SaveChangesAsync();
//            }

//            return RedirectToAction("List", "Tasks");
//        }
//    }
//}

using compiti.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using compiti.Data;
using compiti.Models.DTO;

namespace compiti.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompitoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CompitoController(ApplicationDbContext context)
        {
            _context = context;
        }


    }
}
