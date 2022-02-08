using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Dynamic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
// TODO Lesson03 1-1: import namespaces Lesson03.Models and Microsoft.EntityFrameworkCore
// Un-comment the following two lines:
 using Lesson03.Models;
 using Microsoft.EntityFrameworkCore;

namespace Lesson03.Controllers
{
    public class MugOrderController : Controller
    {
        // TODO Lesson03 1-2: Create a private variable _dbContext of type AppDbContext class
        private AppDbContext _dbContext;
        // TODO Lesson03 1-3: Create constructor to receive dbContext and initialize the _dbContext variable
        public MugOrderController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [Authorize]
        public IActionResult Index()
        {
            // TODO Lesson03 1-5: Set dbs to property dbSet for mug orders
             DbSet<MugOrder> dbs = _dbContext.MugOrder;


            // TODO Lesson03 1-6: Set model to all records of mug orders - use dbs.ToList()
             List<MugOrder> model = dbs.ToList();

            // TODO Lesson03 1-7: If current logged in user is not Admin then filter the list using CreatedBy column
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (!User.IsInRole("Admin"))
                model = model.Where(m => m.CreatedBy == userId).ToList();

            // TODO Lesson03 1-8: Pass model to view
            return View(model); // this line needs to be modified
        }

        [Authorize]
        public IActionResult Create()
        {
            // TODO Lesson03 2-4: Use ToList<Pokedex> to retrieve all pokedex
            // where value is Id and text is Name and pass the list to the view through ViewData
            DbSet<Pokedex> dbs = _dbContext.Pokedex;
            var lstPokes = dbs.ToList();

            // TODO Lesson03 2-5: Prepare a SelectList to for drop down list to select pokedex in the view. Pass to ViewData["poke"]
            ViewData["poke"] = new SelectList(lstPokes, "Id", "Name");
            return View();
        }

        [Authorize]
        [HttpPost]
        // TODO Lesson03 2-6: Change the data type of mugOrder parameter from dynamic to MugOrder
        public IActionResult Create(dynamic mugOrder)
        {
            if (ModelState.IsValid)
            {
                // TODO Lesson03 2-7: Update the CreatedBy with value of current logged in user id
                var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                mugOrder.CreatedBy = userId;
                // TODO Lesson03 2-8: Add new mug order to database using _dbContext and display appropriate message 
                DbSet<MugOrder> dbs = _dbContext.MugOrder;
                dbs.Add(mugOrder);

                if (_dbContext.SaveChanges() == 1)
                    TempData["Msg"] = "order has been added.";
                else
                    TempData["Msg"] = "unable to update database.";
            }
            else
                TempData["Msg"] = "invalid info";

            return RedirectToAction("Index");
        }

    }
}
// 19047572 Konada Obadiah Nahshon 