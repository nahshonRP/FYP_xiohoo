using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Lesson12.Models;

namespace Lesson12.Controllers
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

            DbSet<Topic> dbsTopic = _dbContext.Topic;
            var topics = dbsTopic.Where(t => t.ModuleId == model[0].ModuleId)
                                 .OrderBy(t => t.TopicId)
                                 .ToList();
            ViewData["topics"] = new SelectList(topics, "TopicId", "Title");

            return View(model);
        }

        public IActionResult ListByModule(string moduleId)
        {
            DbSet<Module> dbs = _dbContext.Module;
            var model = dbs.Where(m => m.ModuleId == moduleId)
                            .Include(m => m.Lesson)
                            .ToList();

            DbSet<Topic> dbsTopic = _dbContext.Topic;
            var topics = dbsTopic.Where(t => t.ModuleId == model[0].ModuleId)
                                 .OrderBy(t => t.TopicId)
                                 .ToList();
            ViewData["topics"] = new SelectList(topics, "TopicId", "Title");

            return View("Index",model);
        }

        public IActionResult ListByModuleLesson(string moduleId, int frLessonId, int toLessonId)
        {
            DbSet<Module> dbs = _dbContext.Module;
            var model = dbs.Where(m => m.ModuleId == moduleId)
                            .Include(m => m.Lesson)
                            .ToList();

            if (toLessonId < frLessonId)
                toLessonId = frLessonId;

            model[0].Lesson = model[0].Lesson
                                      .Where(l => l.LessonId >= frLessonId && l.LessonId <= toLessonId)
                                      .ToList();

            DbSet<Topic> dbsTopic = _dbContext.Topic;
            var topics = dbsTopic.Where(t => t.ModuleId == model[0].ModuleId)
                                 .OrderBy(t => t.TopicId)
                                 .ToList();
            ViewData["topics"] = new SelectList(topics, "TopicId", "Title");

            return View("Index", model);
        }

        public IActionResult TopicalIndex()
        {
            DbSet<Module> dbs = _dbContext.Module;
            var model = dbs.Include(m => m.Topic)
                            .ThenInclude(t => t.Lesson)
                            .ToList();

            DbSet<Topic> dbsTopic = _dbContext.Topic;
            var topics = dbsTopic.Where(t => t.ModuleId == model[0].ModuleId)
                                 .OrderBy(t => t.TopicId)
                                 .ToList();
            ViewData["topics"] = new SelectList(topics, "TopicId", "Title");
            ViewData["byTopic"] = true;
            return View("Index", model);
        }

        public IActionResult Search(string keyPhrase)
        {
            DbSet<Lesson> dbsLesson = _dbContext.Lesson;
            var lessons = dbsLesson.Where(l => l.Notes.Contains(keyPhrase))
                                   .Include(l => l.Module)
                                   .ToList();

            var model = lessons.GroupBy(l => l.Module)
                                .Select(g => g.Key)
                                .OrderBy(m => m.ModuleId)
                                .ToList();

            foreach (Module m in model)
            {
                m.Lesson = lessons.Where(l => l.ModuleId == m.ModuleId)
                                  .ToList();
            }

            DbSet<Topic> dbsTopic = _dbContext.Topic;
            var topics = dbsTopic.Where(t => t.ModuleId == model[0].ModuleId)
                                 .OrderBy(t => t.TopicId)
                                 .ToList();

            return View("Index", model);
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