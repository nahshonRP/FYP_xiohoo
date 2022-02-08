using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lesson09.Models;

namespace Lesson09.Controllers
{
    public class RPNotesController : Controller
    {
        private AppDbContext _dbContext;

        public RPNotesController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            DbSet<Module> dbs = _dbContext.Module;
            var model = dbs.Include(m => m.Lesson)
                            .ToList();

            return View(model);
        }

        [HttpPost]
        public IActionResult SaveNotes(Lesson ulesson)
        {
            if (ModelState.IsValid)
            {
                DbSet<Lesson> dbs = _dbContext.Lesson;
                var tLesson = dbs.Where(l => l.ModuleId == ulesson.ModuleId
                                    && l.LessonId == ulesson.LessonId)
                                .FirstOrDefault();
                if (tLesson != null)
                {
                    tLesson.Notes = ulesson.Notes == null ? "" : ulesson.Notes;
                    if (_dbContext.SaveChanges() == 1)
                        TempData["Msg"] = "notes have been saved!";
                    else
                        TempData["Msg"] = "failed to save notes!";
                }
                else
                    TempData["Msg"] = "notes have not found!";
            }
            else
                TempData["Msg"] = "invalid data entry!";
            return RedirectToAction("Index");
        }

        // TODO Lesson09 Solution Task: Complete the GetModules to return partial view _ModulesNav with correct model.
        public IActionResult GetModules()
        {
            DbSet<Module> dbs = _dbContext.Module;
            var modules = dbs.Include(m => m.Lesson).ToList();

            return PartialView("_ModulesNav", modules);
        }
    }
}
//19047572 Konada Obadiah Nahshon