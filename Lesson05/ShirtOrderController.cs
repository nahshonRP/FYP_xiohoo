using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Lesson05.Models;
using System.Dynamic;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Lesson05.Controllers
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
            // modify this code so that only admin see all orders and normal users only see their own orders
            DbSet<ShirtOrder> dbs = _dbContext.ShirtOrder;
            List<ShirtOrder> model = null;
            if (User.IsInRole("Admin"))
                model = dbs.ToList();
            else
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                model = dbs.Where(so => so.CreatedBy == userId).ToList();
            }
            return View(model);
        }

        [Authorize]
        public IActionResult Create()
        {
            DbSet<Pokedex> dbsPokes = _dbContext.Pokedex;
            var lstPokes = dbsPokes.ToList();
            ViewData["pokes"] = new SelectList(lstPokes, "Id", "Name");

            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult Create(ShirtOrder shirtOrder)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                shirtOrder.CreatedBy = userId;
                _dbContext.ShirtOrder.Add(shirtOrder);
                _dbContext.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(shirtOrder);
        }

        [Authorize]
        public IActionResult Update(int id)
        {
            DbSet<ShirtOrder> dbs = _dbContext.ShirtOrder;
            ShirtOrder tOrder = dbs.Where(mo => mo.Id == id).FirstOrDefault();

            if (tOrder != null)
            {
                DbSet<Pokedex> dbsPokes = _dbContext.Pokedex;
                var lstPokes = dbsPokes.ToList();
                ViewData["pokes"] = new SelectList(lstPokes, "Id", "Name");

                return View(tOrder);
            }
            else
            {
                TempData["Msg"] = "shirt order not found!";
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
                ShirtOrder tOrder = dbs.Where(mo => mo.Id == shirtOrder.Id).FirstOrDefault();

                if (tOrder != null)
                {
                    tOrder.Name = shirtOrder.Name;
                    tOrder.Color = shirtOrder.Color;
                    tOrder.PokedexId = shirtOrder.PokedexId;
                    tOrder.Qty = shirtOrder.Qty;
                    tOrder.Price = shirtOrder.Price;
                    tOrder.FrontPosition = shirtOrder.FrontPosition;

                    if (_dbContext.SaveChanges() == 1)
                        TempData["Msg"] = "shirt order has been updated!";
                    else
                        TempData["Msg"] = "update to database has failed!";
                }
                else
                {
                    TempData["Msg"] = "shirt order not found!";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                TempData["Msg"] = "invalid info!";
            }
            return RedirectToAction("Index");
        }


        // TODO Lesson05 Task Solution - Implement Delete action action
        [Authorize]
        public IActionResult Delete(int id)
        {
            DbSet<ShirtOrder> dbs = _dbContext.ShirtOrder;

            ShirtOrder sOrder = dbs.Where(so => so.Id == id).FirstOrDefault();

            if (sOrder != null)
            {
                dbs.Remove(sOrder);

                if (_dbContext.SaveChanges() == 1)
                    TempData["Msg"] = "shirt order deleted!";
                else
                    TempData["Msg"] = "update to database has failed!";
            }
            else
            {
                TempData["Msg"] = "shirt order not found!";
            }
            return RedirectToAction("Index");
        }
    }
}
//19047572 Konada Obadiah Nahshon