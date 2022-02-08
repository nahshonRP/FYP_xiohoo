using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Lesson10.Models;

namespace Lesson10.Controllers
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

            // TODO P10 Solution Task: Complete Index action so that a select list of topics are passed into the view
            DbSet<Topic> dbsTopic = _dbContext.Topic;
            var topics = dbsTopic.Where(t => t.ModuleId == model[0].ModuleId).OrderBy(t => t.TopicId).ToList();

            ViewData["topics"] = new SelectList(topics, "TopicId", "Title");


            return View(model);
        }

        public IActionResult GetModules()
        {
            DbSet<Module> dbs = _dbContext.Module;
            var modules = dbs.Include(m => m.Lesson)
                             .ToList();
            return PartialView("_ModulesNav", modules);
        }

        public IActionResult GetNotes(string moduleId, int lessonId)
        {
            DbSet<Lesson> dbs = _dbContext.Lesson;
            var lesson = dbs.Where(l => l.ModuleId == moduleId && l.LessonId == lessonId).Include(l => l.Module).FirstOrDefault();

            DbSet<Topic> dbsTopic = _dbContext.Topic;
            var topics = dbsTopic.Where(t => t.ModuleId == moduleId)
                                 .OrderBy(t => t.TopicId)
                                 .ToList();
            ViewData["topics"] = new SelectList(topics, "TopicId", "Title");
            return PartialView("_Note", lesson);
        }
    }
}
//19047572 Konada Obadiah Nahshon