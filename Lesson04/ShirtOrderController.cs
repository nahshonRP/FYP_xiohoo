using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Lesson04.Models;
using System.Dynamic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Lesson04.Controllers
{
    public class ShirtOrderController : Controller
    {
        private AppDbContext _dbContext;

        public ShirtOrderController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [Authorize]
        public IActionResult Index()
        {
            DbSet<ShirtOrder> dbs = _dbContext.ShirtOrder;
            List<ShirtOrder> model = dbs.ToList();
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (!User.IsInRole("Admin"))
                model = model.Where(so => so.CreatedBy == userId).ToList();
            return View(model);
        }

        [Authorize]
        public IActionResult Create()
        {
            var dbs = _dbContext.Pokedex;
            var lstPokes = dbs.ToList<Pokedex>();
            ViewData["pokes"] = new SelectList(lstPokes, "Id", "Name");

            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult Create(ShirtOrder shirtOrder)
        {
            if (ModelState.IsValid)
            {
                shirtOrder.CreatedBy = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                _dbContext.ShirtOrder.Add(shirtOrder);

                if (_dbContext.SaveChanges() == 1)
                    TempData["Msg"] = "order added";
                else
                    TempData["Msg"] = "unable to update database";
            }
            else
            {
                TempData["Msg"] = "invalid info";
            }

            return RedirectToAction("Index");
        }

        // Update actions yet to be implemented
        [Authorize]
        public IActionResult Update(int id)
        {
            DbSet<ShirtOrder> dbs = _dbContext.ShirtOrder;

            ShirtOrder tOrder = dbs.Where(so => so.Id == id).FirstOrDefault();

            if (tOrder != null)
            {
                var dbsPokes = _dbContext.Pokedex;
                var lstPokes = dbsPokes.ToList<Pokedex>();
                ViewData["pokes"] = new SelectList(lstPokes, "Id", "Name");
                return View(tOrder);
            }
            else
            {
                TempData["Msg"] = "order not found";
                return RedirectToAction("Index");
            }
        }

        [Authorize]
        [HttpPost]
        public IActionResult Update(ShirtOrder shirtOrder)
        {
            if (ModelState.IsValid)
            {
                DbSet<ShirtOrder> dbs = _dbContext.ShirtOrder;
                ShirtOrder tOrder = dbs.Where(so => so.Id == shirtOrder.Id).FirstOrDefault();

                if (tOrder != null)
                {
                    tOrder.Color = shirtOrder.Color;
                    tOrder.PokedexId = shirtOrder.PokedexId;
                    tOrder.Qty = shirtOrder.Qty;

                    if (_dbContext.SaveChanges() == 1)
                        TempData["Msg"] = "update successful";
                    else
                        TempData["Msg"] = "unable to update database";
                }
                else
                {
                    TempData["Msg"] = "order not found";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                TempData["Msg"] = "invalid info";
            }
            return RedirectToAction("Index");
        }
    }
}
//19047572 Konada Obadiah Nahshon

