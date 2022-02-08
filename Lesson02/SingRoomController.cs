using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Lesson02.Models;
using Microsoft.AspNetCore.Http;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Dynamic;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Lesson02.Controllers
{
    // TODO Lesson02 Task 1a: Use Authorize attribute to configure SingRoom 
    // such that it uses "SingRoom" authentication scheme
    [Authorize(AuthenticationSchemes = "SingRoom")]
    public class SingRoomController : Controller
    {
        // TODO Lesson02 Task 1b: Modify the authorization requirement 
        // so that users with different roles can access different actions.

        // Requirement: All roles can access
        [Authorize(Roles = "Admin,User")]
        public IActionResult Index()
        {
            string userid = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            List<SRBooking> model = DBUtl.GetList<SRBooking>("SELECT * FROM SRBooking WHERE BookedBy = {0}", userid);
            return View(model);
        }

        // Requirement: All roles can access
        [Authorize(Roles = "Admin,User")]
        public IActionResult BookingCreate()
        {
            var packageTypes = DBUtl.GetList("SELECT Id, Description FROM SRPackageType ORDER BY Description");
            ViewData["PackageTypes"] = new SelectList(packageTypes, "Id", "Description");

            var slots = DBUtl.GetList("SELECT Id, Description FROM SRSlot ORDER BY Description");
            ViewData["Slots"] = new SelectList(slots, "Id", "Description");

            return View("BookingCreate");
        }

        // Requirement: All roles can access
        [Authorize(Roles = "Admin,User")]
        [HttpPost]
        public IActionResult BookingCreate(SRBooking newSRBooking)
        {
            string userid = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (ModelState.IsValid)
            {
                if (DBUtl.ExecSQL(@"INSERT INTO SRBooking (Name, SlotId, PackageTypeId, BookingDate, Hours, AOSnack, AODrink,BookedBy) 
                                    VALUES ('{0}', {1}, {2}, '{3}', {4}, '{5}', '{6}',{7})",
                                    newSRBooking.Name, newSRBooking.SlotId, newSRBooking.PackageTypeId, $"{newSRBooking.BookingDate:dd MMMM yyyy}", newSRBooking.Hours, newSRBooking.AOSnack, newSRBooking.AODrink, userid) == 1)
                    TempData["Msg"] = "New booking added.";
                else
                    TempData["Msg"] = DBUtl.DB_Message;
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Msg"] = "Invalid information entered!";
                return RedirectToAction("Index");
            }
        }

        // Requirement: All roles can access
        [Authorize(Roles = "Admin,User")]
        public IActionResult BookingUpdate(int Id)
        {
            var packageTypes = DBUtl.GetList("SELECT Id, Description FROM SRPackageType ORDER BY Description");
            ViewData["PackageTypes"] = new SelectList(packageTypes, "Id", "Description");

            var slots = DBUtl.GetList("SELECT Id, Description FROM SRSlot ORDER BY Description");
            ViewData["Slots"] = new SelectList(slots, "Id", "Description");

            string userid = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            List<SRBooking> lstBooking = DBUtl.GetList<SRBooking>("SELECT * FROM SRBooking WHERE Id = {0} AND BookedBy={1}", Id, userid);
            SRBooking model = null;
            if (lstBooking.Count > 0)
            {
                model = lstBooking[0];
                return View("BookingUpdate", model);
            }
            else
            {
                TempData["Msg"] = $"Booking {Id} not found!";
                return RedirectToAction("Index");
            }
        }

        // Requirement: All roles can access
        [Authorize(Roles = "Admin,User")]
        [HttpPost]
        public IActionResult BookingUpdate(SRBooking uBook)
        {
            if (ModelState.IsValid)
            {
                string userid = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                if (DBUtl.ExecSQL(@"UPDATE SRBooking 
                                    SET Name='{0}', SlotId={1}, PackageTypeId={2}, BookingDate='{3}', 
                                      Hours={4}, AOSnack='{5}', AODrink = '{6}'
                                    WHERE Id = {7} AND BookedBy={8}",
                                    uBook.Name, uBook.SlotId, uBook.PackageTypeId, $"{uBook.BookingDate:dd MMMM yyyy}", uBook.Hours, uBook.AOSnack, uBook.AODrink, uBook.Id, userid) == 1)
                    TempData["Msg"] = $"Booking {uBook.Id} updated.";
                else
                    TempData["Msg"] = DBUtl.DB_Message;
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Msg"] = "Invalid information entered!";
                return RedirectToAction("Index");
            }
        }

        // Requirement: All roles can access
        [Authorize(Roles = "Admin,User")]
        public IActionResult BookingDelete(int Id)
        {
            var packageTypes = DBUtl.GetList("SELECT Id, Description FROM SRPackageType ORDER BY Description");
            ViewData["PackageTypes"] = new SelectList(packageTypes, "Id", "Description");

            var slots = DBUtl.GetList("SELECT Id, Description FROM SRSlot ORDER BY Description");
            ViewData["Slots"] = new SelectList(slots, "Id", "Description");

            string userid = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            List<SRBooking> lstBooking = DBUtl.GetList<SRBooking>("SELECT * FROM SRBooking WHERE Id = {0} AND BookedBy={1}", Id, userid);
            SRBooking model = null;
            if (lstBooking.Count > 0)
            {
                model = lstBooking[0];
                return View("BookingDelete", model);
            }
            else
            {
                TempData["Msg"] = $"Booking {Id} not found!";
                return RedirectToAction("Index");
            }
        }

        // Requirement: All roles can access
        [HttpPost]
        [Authorize(Roles = "Admin,User")]
        public IActionResult BookingDelete(SRBooking uBook)
        {
            string userid = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (DBUtl.ExecSQL(@"DELETE SRBooking 
                                    WHERE Id = {0} AND BookedBy={1}",
                                uBook.Id, userid) == 1)
                TempData["Msg"] = $"Booking {uBook.Id} deleted.";
            else
                TempData["Msg"] = DBUtl.DB_Message;
            return RedirectToAction("Index");
        }

        // Requirement: Only Admin can access
        [Authorize(Roles = "Admin")]
        public IActionResult ViewBookingsByPackage()
        {
            string userid = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            ViewData["PackageTypes"] = DBUtl.GetList("SELECT * FROM SRPackageType ORDER BY Description");
            List<SRBooking> model = DBUtl.GetList<SRBooking>("SELECT * FROM SRBooking WHERE BookedBy = {0}", userid);
            return View(model);
        }

        // Requirement: Only Admin can access
        [Authorize(Roles = "Admin")]
        public IActionResult SalesSummary(int ryear, int rmonth)
        {
            List<SRBooking> data = null;

            ViewData["ryear"] = ryear;
            ViewData["rmonth"] = rmonth;

            if (ryear <= 0)
            {
                data = DBUtl.GetList<SRBooking>(@"SELECT * FROM SRBooking");
                ViewData["reportheader"] = "Overall Sales Summary by Year";

                // TODO Lesson02 Task 2a: Use LINQ query provided in worksheet 
                // to retrieve summary data grouped by Year
                // Note: delete the original List<dynamic> model = null statement
                var model = data.GroupBy(b => b.BookingDate.Year)
                    .OrderByDescending(g => g.Key)
                    .Select(g => new
                    {
                        Group = g.Key,
                        Total = g.Sum(b => b.Cost),
                        Average = g.Average(b => b.Cost),
                        Lowest = g.Min(b => b.Cost),
                        Highest = g.Max(b => b.Cost)
                    }).ToExpandoList();
                
         

                
                // TODO Lesson02 Task 2b: Verification 
                // Run SingRoom, login as admin user and click on the Reports Summary 
                // to ensure that the report will appear now

                return View(model);
            }
            else if (rmonth <= 0 || rmonth > 12)
            {
                data = DBUtl.GetList<SRBooking>(@"SELECT * FROM SRBooking
                                                            WHERE YEAR(BookingDate) = {0}", ryear);
                ViewData["reportheader"] = $"Annual Sales Summary for {ryear} by Month";
                
                // TODO Lesson02 Task 2c: Use LINQ query provided in worksheet 
                // to retrieve summary data grouped by Month for a given year
                // Note: delete the original List<dynamic> model = null statement

                var model = data.GroupBy(b => b.BookingDate.Month)
                    .OrderByDescending(g => g.Key)
                    .Select(g => new
                    {
                        Group = g.Key,
                        Total = g.Sum(b => b.Cost),
                        Average = g.Average(b => b.Cost),
                        Lowest = g.Min(b => b.Cost),
                        Highest = g.Max(b => b.Cost)
                    }).ToExpandoList();

                // TODO Lesson02 Task 2d: Verification 
                // Run SingRoom, login as admin user and click on the Reports Summary 
                // and click on year 2016 to view sales summary by month for year 2016

                return View(model);
            }
            else
            {
                data = DBUtl.GetList<SRBooking>(@"SELECT * FROM SRBooking
                                                            WHERE YEAR(BookingDate) = {0}
                                                                    AND MONTH(BookingDate) = {1}",
                                                                    ryear, rmonth);
                ViewData["reportheader"] = $"Monthly Sales for {ryear} Month {rmonth} by Day";

                // TODO Lesson02 Task 2e: modify this line to get the proper result
                var model = data.GroupBy(b => b.BookingDate.Day)
                    .OrderByDescending(g => g.Key)
                    .Select(g => new
                    {
                        Group = g.Key,
                        Total = g.Sum(b => b.Cost),
                        Average = g.Average(b => b.Cost),
                        Lowest = g.Min(b => b.Cost),
                        Highest = g.Max(b => b.Cost)
                    }).ToExpandoList();

                // TODO Lesson02 Task 2f: Verification 
                // Run SingRoom, login as admin user and click on the Reports Summary
                // and click on 2016, then click on any month to view sales summary by day

                return View(model);
            }
        }

        // Requirement: Only Admin can access
        [Authorize(Roles = "Admin")]
        public IActionResult Reports()
        {
            return View();
        }

        // Requirement: Public can access
        [AllowAnonymous]
        public IActionResult AboutUs()
        {
            return View();
        }
    }
}// 19047572 Konada Obadiah Nahshon

