using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Lesson08.Models;

namespace Lesson08.Controllers
{
    public class HomeController : Controller
    {
        private AppDbContext _dbContext;

        public HomeController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult PPUExercise1()
        {
            return View();
        }

        public IActionResult PPULoadPageEx1()
        {
            // TODO Lesson08 Ex1a Return the partial view _Ex1.
            // Navigate to Home/PPULoadPageEx1 to test the partial view
            PPUExercise1 model = new PPUExercise1();
            return PartialView("_Ex1", model);
        }

        public IActionResult PPUExercise2()
        {
            return View();
        }

        public IActionResult PPULoadPageEx2(string word)
        {
            String reversedWord = new string(word.Trim().Reverse().ToArray<char>());

            // TODO Lesson08 Ex2a Return the partial view _Ex2, use reversedWord variable as the model
            // Navigate to Home/PPULoadPageEx2?word=hello to test the partial view
            return PartialView("_Ex2", reversedWord);
        }

        public IActionResult CreateMugOrder()
        {
            DbSet<Pokedex> dbs = _dbContext.Pokedex;
            var lstPokes = dbs.ToList();
            ViewData["pokes"] = new SelectList(lstPokes, "Id", "Name");
            return View();
        }

        public IActionResult LoadMug(string color, int pokedexid)
        {
            // TODO Lesson08 Ex3a Complete the following code to create a new MugOrder and assign the pokedexid and color to the model properties
            MugOrder model = new MugOrder();
            model.PokedexId = pokedexid;
            model.Color = color;

            // TODO Lesson08 Ex3b Use PartialView method to return _ShowMug partial view and pass model object as the model
            // Navigate to Home/LoadMug?color=silver&pokedexid=77 to test the partial view
            return PartialView("_ShowMug", model);
        }
    }
}
//19047572 Konada Obadiah Nahshon