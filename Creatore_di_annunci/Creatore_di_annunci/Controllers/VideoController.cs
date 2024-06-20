using Creatore_di_annunci.Data;
using Creatore_di_annunci.Models;
using Creatore_di_annunci.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using System.Diagnostics;
using OpenAI;
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;
using System.Threading.Tasks;
using System.Text;


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

            if (string.IsNullOrEmpty(estensione) || !estensioni.Contains(estensione))
            {
                ModelState.AddModelError("VideoFile", "Invalid file type.");
                return View(viewModel);
            }

            var video = new Video
            {
                Status = 0,
                Description = "",
                Path = "",
                json = "",
                Annuncio = ""
            };

            await dbContext.Videos.AddAsync(video);
            await dbContext.SaveChangesAsync();

            var newFileName = $"{video.Id}{estensione}";
            var uploadPath = Path.Combine(@"C:\Users\loris\Documents\GitHub\STAGE\Creatore_di_annunci\Videos", newFileName);

            Directory.CreateDirectory(Path.GetDirectoryName(uploadPath));

            string newpath = Path.Combine(@"C:\Users\loris\Documents\GitHub\STAGE\Creatore_di_annunci\Videos", filePath);

            try
            {
                System.IO.File.Copy(newpath, uploadPath, true);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("VideoFile", $"Error copying file: {ex.Message}");
                return View(viewModel);
            }

            System.IO.File.Delete(newpath);

            video.Path = uploadPath;
            dbContext.Videos.Update(video);
            await dbContext.SaveChangesAsync();

            var scriptPath = @"C:\Users\loris\Documents\GitHub\STAGE\Creatore_di_annunci\Videos\From_Video_To_Text.py";

            if (!System.IO.File.Exists(scriptPath))
            {
                ModelState.AddModelError("", "Script file not found.");
                return View(viewModel);
            }

            var processStartInfo = new ProcessStartInfo
            {
                FileName = "python",
                Arguments = $"\"{scriptPath}\" \"{uploadPath}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            try
            {
                using (var process = new Process { StartInfo = processStartInfo })
                {
                    var output = new StringBuilder();
                    var error = new StringBuilder();

                    process.OutputDataReceived += (sender, e) =>
                    {
                        if (e.Data != null)
                        {
                            output.AppendLine(e.Data);
                        }
                    };

                    process.ErrorDataReceived += (sender, e) =>
                    {
                        if (e.Data != null)
                        {
                            error.AppendLine(e.Data);
                        }
                    };

                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    // Aumentare il timeout a 30 minuti (1800000 millisecondi)
                    if (!process.WaitForExit(1600000))
                    {
                        process.Kill();
                        ModelState.AddModelError("", "Script execution timed out.");
                        return View(viewModel);
                    }

                    if (process.ExitCode != 0)
                    {
                        ModelState.AddModelError("", $"Error executing script: {error}");
                        return View(viewModel);
                    }

                    string descrizione = output.ToString().Trim();
                    await GeneraAnnunncio(descrizione, video.Id, dbContext);

                    return RedirectToAction("List", "Video");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Exception during script execution: {ex.Message}");
                return View(viewModel);
            }
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
            var house = await dbContext.houses.FindAsync(id);

            if (video != null)
            {
                dbContext.Videos.Remove(video);
                dbContext.houses.Remove(house);
                await dbContext.SaveChangesAsync();
            }

            return RedirectToAction("List", "Video");
        }

        public static async Task GeneraAnnunncio(string descrizione, Guid id, ApplicationDbContext dbContext)
        {
            // Recupera il video dal database utilizzando l'ID
            var video = await dbContext.Videos.FindAsync(id);
            if (video == null)
            {
                // Gestisci il caso in cui il video non viene trovato
                throw new Exception("Video non trovato");
            }

            video.Description = descrizione;
            video.Status = 3; // Stato aggiornato a "in creazione"
            dbContext.Videos.Update(video);
            await dbContext.SaveChangesAsync();

            // Inizializza l'API OpenAI
            //var api = new OpenA
            //    IAPI(new A
            //    PIAuthentication("sk-VBfcEDlhSO07nc" +
            //    "Wm1ldYT3BlbkFJNKnUev9xIKrS38stdAqh"));

            // Descrizione dell'appartamento
            string description = video.Description;

            if (string.IsNullOrEmpty(description))
            {
                throw new Exception("Descrizione è nulla o vuota");
            }

            // Creazione del messaggio da inviare
            var chat = api.Chat.CreateConversation();
            chat.Model = Model.GPT4_Turbo;
            chat.RequestParameters.Temperature = 0;

            // Invia il messaggio di sistema e dell'utente
            chat.AppendSystemMessage("creami un annuncio dato il l'immobile fornito ottimizzato per la ceo, con linguaggio accattivante, che metta in risalto le parole chiave. cerca dove possibile di mettere le dimensioni specifiche delle varie stanze");
            chat.AppendUserInput(description);

            // Ottieni la risposta dal chatbot
            string response = await chat.GetResponseFromChatbotAsync();

            // Assegna l'annuncio generato al video
            video.Annuncio = response;

            // Aggiorna lo stato a 4 dopo la generazione dell'annuncio
            video.Status = 4;
            dbContext.Videos.Update(video);
            await dbContext.SaveChangesAsync();

            await GeneraJson(response, id, dbContext);
        }

        public static async Task GeneraJson(string annuncio, Guid id, ApplicationDbContext dbContext)
        {
            // Recupera il video dal database utilizzando l'ID
            var video = await dbContext.Videos.FindAsync(id);
            if (video == null)
            {
                // Gestisci il caso in cui il video non viene trovato
                throw new Exception("Video non trovato");
            }

            if (string.IsNullOrEmpty(annuncio))
            {
                throw new Exception("Annuncio è nullo o vuoto");
            }

            // Imposta lo stato a 5 prima di generare l'annuncio
            video.Status = 5;
            dbContext.Videos.Update(video);
            await dbContext.SaveChangesAsync();

            // Inizializza l'API OpenAI
            //var api = new OpenA
            //    IAPI(new A
            //    PIAuthentication("sk-VBfcEDlhSO07nc" +
            //    "Wm1ldYT3BlbkFJNKnUev9xIKrS38stdAqh"));
            // Creazione del messaggio da inviare
            var chat = api.Chat.CreateConversation();
            chat.Model = Model.GPT4_Turbo;
            chat.RequestParameters.Temperature = 0;

            // Invia il messaggio di sistema e dell'utente
            chat.AppendSystemMessage("Devi crearmi un JSON per un annuncio di un appartamento con i seguenti parametri: Piani, Bagni, MQuadri, ascensore, terrazza, giardino, piscina, prezzo. Se uno di questi parametri non è presente nell'annuncio, usa i seguenti valori predefiniti: Piani=0, Bagni=0, MQuadri=0, ascensore=false ,piscina=false, terrazza=false,giardino=false, prezzo=0. Tutte le altre informazioni non sono necessarie quindi non calcolarle. prendi questo come modello { \"Piani\": 2, \"Bagni\": 2, \"MQuadri\": 0, \"ascensore\": false }");
            chat.AppendUserInput(annuncio);

            // Ottieni la risposta dal chatbot
            string response = await chat.GetResponseFromChatbotAsync();

            // Rimuovi eventuali delimitatori di codice ```json
            response = response.Trim();
            if (response.StartsWith("```json"))
            {
                response = response.Substring(7);
            }
            if (response.EndsWith("```"))
            {
                response = response.Substring(0, response.Length - 3);
            }

            // Assegna l'annuncio generato al video
            video.json = response.Trim();

            // Aggiorna lo stato a 6 dopo la generazione dell'annuncio
            video.Status = 6;
            dbContext.Videos.Update(video);
            await dbContext.SaveChangesAsync();

            await AggiungiHouse(video.json, id, dbContext);

        }

        public static async Task AggiungiHouse(string json, Guid id, ApplicationDbContext dbContext)
        {
            // Deserialize the JSON string to a dynamic object
            var houseData = JsonSerializer.Deserialize<House>(json);

            // Create a new House object and populate it with data from the JSON
            var house = new House
            {
                Id = id,
                Piani = houseData.Piani,
                Bagni = houseData.Bagni,
                MQuadri = houseData.MQuadri,
                ascensore = houseData.ascensore,
                terrazza= houseData.terrazza,
                piscina= houseData.piscina,
                prezzo= houseData.prezzo,
                giardino= houseData.giardino
            };

            // Save the new House object to the database
            await dbContext.AddAsync(house);
            await dbContext.SaveChangesAsync();
        }
    }

}

