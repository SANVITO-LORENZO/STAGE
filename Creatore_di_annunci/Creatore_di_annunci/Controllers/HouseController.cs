using Creatore_di_annunci.Data;
using Creatore_di_annunci.Models.Entities;
using Creatore_di_annunci.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;


namespace Creatore_di_annunci.Controllers
{
    public class HouseController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public HouseController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddHouse viewModel)
        {
            var house = new House
            {
                Bagni= viewModel.Bagni,
                ascensore= viewModel.ascensore,
                MQuadri= viewModel.MQuadri,
                Piani= viewModel.Piani
            };
            await dbContext.houses.AddAsync(house);
            await dbContext.SaveChangesAsync();
            return RedirectToAction("List", "House");
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var video = await dbContext.houses.ToListAsync();
            return View(video);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var houses = await dbContext.houses.FindAsync(id);
            return View(houses);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(House viewModel)
        {
            var house = await dbContext.houses.FindAsync(viewModel.Id);

            if (house is not null)
            {
                house.MQuadri = viewModel.MQuadri;
                house.Piani = viewModel.Piani;
                house.Bagni = viewModel.Bagni;
                house.ascensore = viewModel.ascensore;
                house.giardino = viewModel.giardino;
                house.piscina = viewModel.piscina;
                house.terrazza = viewModel.terrazza;
                house.prezzo = viewModel.prezzo;

                await dbContext.SaveChangesAsync();
            }

            return RedirectToAction("List", "House");
        }
        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var house = await dbContext.houses.FindAsync(id);

            if (house != null)
            {
                dbContext.houses.Remove(house);
                await dbContext.SaveChangesAsync();
            }

            return RedirectToAction("List", "House");
        }


    }
}
