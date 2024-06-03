using Creatore_di_annunci.Data;
using Creatore_di_annunci.Models;
using Creatore_di_annunci.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Creatore_di_annunci.Controllers
{
    public class VideoController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public VideoController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddVideo viewModel)
        {
            var video = new Video
            {
                Path = viewModel.Path,
                Status = 0
            };
            await dbContext.Videos.AddAsync(video);
            await dbContext.SaveChangesAsync();
            return RedirectToAction("List", "Video");
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var video = await dbContext.Videos.ToListAsync();
            return View(video);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var students = await dbContext.Videos.FindAsync(id);
            return View(students);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Video viewModel)
        {
            var video = await dbContext.Videos.FindAsync(viewModel.Id);

            if (video is not null)
            {
                video.Path = viewModel.Path;
                if(video.Status == 0) 
                    video.Status = 1;
                if (video.Status == 1)
                    video.Status = 2;

                await dbContext.SaveChangesAsync();
            }

            return View();
        }
    }
}
