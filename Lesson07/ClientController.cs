using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Lesson07.Models;
using System.Dynamic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

// TODO Lesson07 Revision Task 1: Annotate the Client.cs according the requirements set forth in worksheet

namespace Lesson07.Controllers
{
    public class ClientController : Controller
    {
        private AppDbContext _dbContext = null;

        public ClientController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ClientsIndex()
        {
            // TODO Lesson07 Revision Task 2a: Prepare the model
            //                        Task 2b is in the ClientsIndex.cshtml view
            DbSet<Client> dbsClient = _dbContext.Client;// Replace with proper LINQ query
            List<Client> model = dbsClient.Include(c => c.Package).ToList();

            return View(model);
        }

        // TODO Lesson07 Revision Task 3a: Implement AddNewClient HttpGet action
        // The following AddNewClient HttpGet action is to be replaced by your own implementation
        public IActionResult AddNewClient()
        {
            {
                var dbs = _dbContext.Package;
                var pack1 = dbs.ToList<Package>();
                ViewData["packages"] = new SelectList(pack1, "Id", "Name");

                return View();
            }
        }

        // TODO Lesson07 Revision Task 3b: Create and implement AddNewClient.cshtml view
        public IActionResult UpdateClient(int id)
        {
            DbSet<Client> dbsClient = _dbContext.Client;
            Client client = dbsClient.Where(c => c.Id == id).FirstOrDefault();

            if (client != null)
            {
                DbSet<Package> dbsPackage = _dbContext.Package;
                var lstPackage = dbsPackage.ToList();
                ViewData["packages"] = new SelectList(lstPackage, "Id", "Name");
                return View(client);
            }
            else
            {
                TempData["Msg"] = "unable to find client!";
                return RedirectToAction("ClientsIndex");
            }
        }

        [HttpPost]
        public IActionResult UpdateClient(Client client)
        {
            if (ModelState.IsValid)
            {
                DbSet<Client> dbs = _dbContext.Client;
                Client tOrder = dbs.Where(c => c.Id == client.Id).FirstOrDefault();

                if (tOrder != null)
                {
                    tOrder.Name = client.Name;
                    tOrder.PaymentMode = client.PaymentMode;
                    tOrder.PackageId = client.PackageId;

                    if (_dbContext.SaveChanges() == 1)
                        TempData["Msg"] = "client has been updated!";
                    else
                        TempData["Msg"] = "unable to update client!";
                }
                else
                {
                    TempData["Msg"] = "client not found!";
                    return RedirectToAction("ClientsIndex");
                }
            }
            else
            {
                TempData["Msg"] = "invalid info entered";
            }
            return RedirectToAction("ClientsIndex");
        }


        // TODO Lesson07 Revision Task 3c: Implement AddNewClient HttpPost action

        public IActionResult Create(Client client)
        {
            if (ModelState.IsValid)
            {
                DbSet<Client> dbs = _dbContext.Client;
                dbs.Add(client);
                if (_dbContext.SaveChanges() == 1)
                    TempData["Msg"] = "new client added!";
                else
                    TempData["Msg"] = "unable to add new client!";

            }
            else
            {
                TempData["Msg"] = "invalid info";
            }
            return RedirectToAction("ClientsIndex");
        }
    }
}
//19047572 Konada Obadiah Nahshon
