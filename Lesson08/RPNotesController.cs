using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lesson08.Models;

namespace Lesson08.Controllers
{
    public class RPNotesController : Controller
    {
        // TODO Lesson08 Solution Task: Prepare controller for EF dbContext injection
        private AppDbContext _dbContext;
        public RPNotesController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        // TODO Lesson08 Solution Task: Complete Index action. The model should be the full list of modules and each module can navigate to its associated lessons
        public IActionResult Index()
        {
            DbSet<Module> dbs = _dbContext.Module;
            var model = dbs.Include(m => m.Lesson).ToList();
            return View(model);
        }

        // TODO Lesson08 Solution Task: Complete the SaveNotes HttpPost action
        // Hints: Subtasks to do
        //          1. Only perform saving when ModelState.IsValid is true
        //          2. Search for the target lesson, perform update if the lesson is found
        //          3. Only property needs to update is the Notes property
        //          4. TempData["Msg"]:
        //              - if ModelState is not valid: Invalid data entry
        //              - if lesson not found: Notes not found!
        //              - if update failed: Failed to save notes!
        //              - if update successful: Notes saved!
        [HttpPost]
        public IActionResult SaveNotes(Lesson ulesson)
        {
            if(ModelState.IsValid)
            {
                DbSet<Lesson> dbs = _dbContext.Lesson;
                var tLesson = dbs.Where(l => l.ModuleId == ulesson.ModuleId && l.LessonId == ulesson.LessonId).FirstOrDefault();
                if (tLesson != null)
                {
                    tLesson.Notes = ulesson.Notes;
                    if (_dbContext.SaveChanges() == 1)
                        TempData["Msg"] = "Notes have been saved!";
                    else
                        TempData["Msg"] = "Failed to save notes!";
                }
                else
                    TempData["Msg"] = "Unable to find notes!";
            }
            else
                TempData["Msg"] = "Invalid data entry!";
            return RedirectToAction("Index");
        }

        // TODO Lesson08 Solution Task: Complete the GetNotes to return partial view _Note with correct model.
        public IActionResult GetNotes(string moduleId, int lessonId)
        {
            DbSet<Lesson> dbs = _dbContext.Lesson;
            var lesson = dbs.Where(l => l.ModuleId == moduleId && l.LessonId == lessonId)
                         .Include(l => l.Module).FirstOrDefault();
            return PartialView("_Note", lesson);
        }
    }
}
//19047572 Konada Obadiah Nahshon