using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Lesson06.Models;
using System.Dynamic;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rotativa.AspNetCore;
namespace P06.Controllers
{
    public class CakeOrderController : Controller
    {
        private AppDbContext _dbContext;

        public CakeOrderController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [Authorize]
        public IActionResult Index()
        {

            DbSet<CakeOrder> dbs = _dbContext.CakeOrder;
            List<CakeOrder> model = null;
            if (User.IsInRole("Admin"))
                model = dbs.Include(co => co.Pokedex)
                            .Include(co => co.UserCodeNavigation)
                            .ToList();
            else
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                model = dbs.Where(co => co.UserCode == userId)
                            .Include(co => co.Pokedex)
                            .ToList();
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
        public IActionResult Create(CakeOrder cakeOrder)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                cakeOrder.UserCode = userId;
                _dbContext.CakeOrder.Add(cakeOrder);
                _dbContext.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(cakeOrder);
        }

        [Authorize]
        public IActionResult Update(int id)
        {
            DbSet<CakeOrder> dbs = _dbContext.CakeOrder;
            CakeOrder tOrder = dbs.Where(co => co.Id == id).FirstOrDefault();

            if (tOrder != null)
            {
                DbSet<Pokedex> dbsPokes = _dbContext.Pokedex;
                var lstPokes = dbsPokes.ToList();
                ViewData["pokes"] = new SelectList(lstPokes, "Id", "Name");

                return View(tOrder);
            }
            else
            {
                TempData["Msg"] = "order not found!";
                return RedirectToAction("Index");
            }
        }

        [Authorize]
        [HttpPost]
        public IActionResult Update(CakeOrder cakeOrder)
        {
            if (ModelState.IsValid)
            {
                DbSet<CakeOrder> dbs = _dbContext.CakeOrder;
                CakeOrder tOrder = dbs.Where(co => co.Id == cakeOrder.Id).FirstOrDefault();

                if (tOrder != null)
                {
                    tOrder.Flavor = cakeOrder.Flavor;
                    tOrder.Greeting = cakeOrder.Greeting;
                    tOrder.PokedexId = cakeOrder.PokedexId;
                    tOrder.Qty = cakeOrder.Qty;


                    if (_dbContext.SaveChanges() == 1)
                        TempData["Msg"] = "order has been updated!";
                    else
                        TempData["Msg"] = "unable to update database!";
                }
                else
                {
                    TempData["Msg"] = "order not found!";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                TempData["Msg"] = "invalid info";
            }
            return RedirectToAction("Index");
        }

        [Authorize]
        public IActionResult Delete(int id)
        {
            DbSet<CakeOrder> dbs = _dbContext.CakeOrder;
            CakeOrder tOrder = dbs.Where(co => co.Id == id).FirstOrDefault();

            if (tOrder != null)
            {
                dbs.Remove(tOrder);

                if (_dbContext.SaveChanges() == 1)
                    TempData["Msg"] = "order has been deleted!";
                else
                    TempData["Msg"] = "unable to update database!";
            }
            else
            {
                TempData["Msg"] = "order not found!";
            }
            return RedirectToAction("Index");
        }
        [Authorize]
        public IActionResult PrintOrder(int id)
        {
            DbSet<CakeOrder> dbs = _dbContext.CakeOrder;
            CakeOrder model = dbs.Where(co => co.Id == id)
                                .Include(co => co.Pokedex)
                                .Include(co => co.UserCodeNavigation)
                                .FirstOrDefault();
            if (model != null)
                return new ViewAsPdf(model)
                {
                    PageSize = Rotativa.AspNetCore.Options.Size.B5,
                    PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape
                };
            else
            {
                TempData["Msg"] = "order not found!";
                return RedirectToAction("Index");
            }

        }



    }
}
//19047572 Konada Obadiah Nahshon  
