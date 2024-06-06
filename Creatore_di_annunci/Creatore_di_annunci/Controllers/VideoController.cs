using Creatore_di_annunci.Data;
using Creatore_di_annunci.Models;
using Creatore_di_annunci.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using System.Diagnostics;
using OpenAI_API;
using OpenAI_API.Chat;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            string filePath = viewModel.VideoFile;
            var estensione = Path.GetExtension(filePath).ToLowerInvariant();

            // Controllo se tra le estensioni nel vettore è presente quella del mio file
            if (string.IsNullOrEmpty(estensione) || !estensioni.Contains(estensione))
            {
                ModelState.AddModelError("VideoFile", "Invalid file type.");
                return View(viewModel);
            }

            // Crea nuovo oggetto Video
            var video = new Video
            {
                Status = 0,
                Description = "", // La descrizione sarà aggiornata dopo l'esecuzione dello script
                Path = "",
                json = "",
                Annuncio = ""
            };

            // Salvataggio nel database
            await dbContext.Videos.AddAsync(video);
            await dbContext.SaveChangesAsync();

            // Creazione del nome e estensione del nuovo file
            var newFileName = $"{video.Id}{estensione}";
            var uploadPath = Path.Combine(@"C:\Users\loris\Documents\GitHub\STAGE\Creatore_di_annunci\Videos", newFileName);

            Directory.CreateDirectory(Path.GetDirectoryName(uploadPath));

            // Funziona solo nella cartella Videos
            string newpath = Path.Combine(@"C:\Users\loris\Documents\GitHub\STAGE\Creatore_di_annunci\Videos", filePath);

            try
            {
                // Copia del file
                System.IO.File.Copy(newpath, uploadPath, true);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("VideoFile", $"Error copying file: {ex.Message}");
                return View(viewModel);
            }

            // Rimozione del vecchio file
            System.IO.File.Delete(newpath);

            // Rinomina del path e salvataggio nel database
            video.Path = uploadPath;
            dbContext.Videos.Update(video);
            await dbContext.SaveChangesAsync();

            // Esecuzione dello script Python
            var scriptPath = @"C:\Users\loris\Documents\GitHub\STAGE\Creatore_di_annunci\Videos\From_Video_To_Text.py";
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "python",
                Arguments = $"\"{scriptPath}\" \"{uploadPath}\"", // Passa il percorso del video come argomento
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            string output;
            using (var process = new Process { StartInfo = processStartInfo })
            {
                process.Start();
                output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    ModelState.AddModelError("", $"Error executing script: {error}");
                    return View(viewModel);
                }
            }

            // Imposta la descrizione con l'output dello script
            string descrizione = output.Trim(); // Rimuovi eventuali spazi bianchi

            // Richiama la funzione GeneraAnnunncio passando la descrizione, l'id del video e il dbContext
            await GeneraAnnunncio(descrizione, video.Id, dbContext);

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
                video.Annuncio = viewModel.Annuncio;

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

        static async Task GeneraAnnunncio(string descrizione,Guid id, ApplicationDbContext dbContext)
        {
            // Recupera il video dal database utilizzando l'ID
            var video = await dbContext.Videos.FindAsync(id);
            if (video == null)
            {
                // Gestisci il caso in cui il video non viene trovato
                throw new Exception("Video not found");
            }

            video.Description= descrizione;
            video.Status = 3;
            dbContext.Videos.Update(video);
            await dbContext.SaveChangesAsync();

            // Descrizione dell'appartamento
            string description = video.Description;

            if (string.IsNullOrEmpty(description))
            {
                throw new Exception("Description is null or empty");
            }

            // Creazione del messaggio da inviare
            var message = new ChatMessage()
            {
                Role = ChatMessageRole.User,
                Content = $"Crea un annuncio per questo appartamento: {description}"
            };

            var chatRequest = new ChatRequest
            {
                Model = OpenAI_API.Models.Model.ChatGPTTurbo,  // Specifica il modello da utilizzare
                Messages = new List<ChatMessage> { message }   // Invia il messaggio creato
            };

            // Esegui la richiesta in modo asincrono e ottieni la risposta
            var response = await api.Chat.CreateChatCompletionAsync(chatRequest);

            // Aggiungi l'annuncio generato all'attributo Annuncio
            video.Annuncio = response.Choices.First().Message.Content;

            // Aggiorna lo stato a 4 dopo la generazione dell'annuncio
            video.Status = 4;
            dbContext.Videos.Update(video);
            await dbContext.SaveChangesAsync();
        }
        //static async Task GeneraAnnunncio(Guid id, ApplicationDbContext dbContext)
        //{
        //    // Recupera il video dal database utilizzando l'ID
        //    var video = await dbContext.Videos.FindAsync(id);
        //    if (video == null)
        //    {
        //        // Gestisci il caso in cui il video non viene trovato
        //        throw new Exception("Video not found");
        //    }

        //    // Aggiorna lo stato a 3 all'inizio
        //    video.Status = 3;
        //    dbContext.Videos.Update(video);
        //    await dbContext.SaveChangesAsync();

        //    // Sostituisci 'YOUR_API_KEY' con la tua chiave API reale.
        //    var api = new OpenAIAPI("YOUR_API_KEY");

        //    // Descrizione dell'appartamento
        //    string description = video.Description;

        //    // Creazione del messaggio da inviare
        //    var message = new ChatMessage()
        //    {
        //        Role = ChatMessageRole.User,
        //        Content = $"Crea un annuncio per questo appartamento: {description}"
        //    };

        //    var chatRequest = new ChatRequest
        //    {
        //        Model = OpenAI_API.Models.Model.ChatGPTTurbo,  // Specifica il modello da utilizzare
        //        Messages = new List<ChatMessage> { message }   // Invia il messaggio creato
        //    };

        //    // Esegui la richiesta in modo asincrono e ottieni la risposta
        //    var response = await api.Chat.CreateChatCompletionAsync(chatRequest);

        //    // Aggiungi l'annuncio generato all'attributo Annuncio
        //    video.Annuncio = response.Choices.First().Message.Content;

        //    // Aggiorna lo stato a 4 dopo la generazione dell'annuncio
        //    video.Status = 4;
        //    dbContext.Videos.Update(video);
        //    await dbContext.SaveChangesAsync();
        //}
    }
}
