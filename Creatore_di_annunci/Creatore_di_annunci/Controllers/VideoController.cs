using Creatore_di_annunci.Data;
using Creatore_di_annunci.Models;
using Creatore_di_annunci.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using System.Diagnostics;

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
            var estensioni = new[] { ".mp4", ".mov", ".wmv", ".avi", ".mkv" };
            string FilePath = viewModel.VideoFile;
            var Estensione = Path.GetExtension(FilePath).ToLowerInvariant();

            // CONTROLLO SE TRA LE ESTENSIONI NEL VETTORE E' PRESENTE QUELLA DEL MIO FILE
            if (string.IsNullOrEmpty(Estensione) || !estensioni.Contains(Estensione))
            {
                ModelState.AddModelError("VideoFile", "Invalid file type.");
                return View(viewModel);
            }

            // CREO NUOVO OGGETTO VIDEO
            var video = new Video
            {
                Status = 0,
                Description = "",
                Path = "",
                json=""
            };

            // SALVATAGGIO NEL DATABASE
            await dbContext.Videos.AddAsync(video);
            await dbContext.SaveChangesAsync();

            // CREAZIONE DEL NOME E ESTENSIONE DEL NUOVO FILE
            var newFileName = $"{video.Id}{Estensione}";
            var uploadPath = Path.Combine(@"C:\Users\loris\Documents\GitHub\STAGE\Creatore_di_annunci\Videos", newFileName);

            Directory.CreateDirectory(Path.GetDirectoryName(uploadPath));

            // FUNZIONER' SOLO NELLA CARTELLA VIDEOS
            string newpath = @"C:\Users\loris\Documents\GitHub\STAGE\Creatore_di_annunci\Videos\" + FilePath;

            try
            {
                // COPIA DEL FILE
                System.IO.File.Copy(newpath, uploadPath, true);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("VideoFile", $"Error copying file: {ex.Message}");
                return View(viewModel);
            }

            // RIMOZIONE DEL VECCHIO FILE
            System.IO.File.Delete(newpath);

            // RINOMINAZIONE DEL PATH E SALVATAGGIO NEL DATABASE
            video.Path = uploadPath;
            dbContext.Videos.Update(video);
            await dbContext.SaveChangesAsync();

            RedirectToAction("List", "Video");

            // ESECUZIONE DELLO SCRIPT PYTHON
            var scriptPath = @"C:\Users\loris\Documents\GitHub\STAGE\Creatore_di_annunci\Videos\From_Video_To_Text.py";
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "python",
                Arguments = $"\"{scriptPath}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = new Process { StartInfo = processStartInfo })
            {
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    ModelState.AddModelError("", $"Error executing script: {error}");
                    return View(viewModel);
                }
            }

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
            var videos = await dbContext.Videos.FindAsync(id);
            return View(videos);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Video viewModel)
        {
            var video = await dbContext.Videos.FindAsync(viewModel.Id);

            if (video is not null)
            {
                video.Path = viewModel.Path;
                video.Description = viewModel.Description;
                video.Status = viewModel.Status;
                video.json = viewModel.json;

                await dbContext.SaveChangesAsync();
            }

            return RedirectToAction("List", "Video");
        }
        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var video = await dbContext.Videos.FindAsync(id);

            if (video != null)
            {
                dbContext.Videos.Remove(video);
                await dbContext.SaveChangesAsync();
            }

            return RedirectToAction("List", "Video");
        }

    }
}
