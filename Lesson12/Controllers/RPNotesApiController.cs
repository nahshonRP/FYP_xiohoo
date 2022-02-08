using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lesson12.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Lesson12.Controllers
{
    [Route("api/RPNotes")]
    public class RPNotesApiController : Controller
    {
        private AppDbContext _dbContext;
        private string _apiKey = "uQpz8en9mbaUYF958sKmKbwtWCzXBt";

        public RPNotesApiController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("Modules")]
        public IActionResult GetAllModules()
        {
            DbSet<Module> dbs = _dbContext.Module;
            var modules = dbs.OrderBy(m => m.ModuleId).ToList();
            return Ok(modules);
        }

        [HttpPost("Save")]
        public IActionResult UpdateNotes(Lesson ulesson)
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
                    tLesson.TopicId = ulesson.TopicId;
                    if (_dbContext.SaveChanges() == 1)
                        return Ok(1);
                    else
                        return Ok(0);
                }
                else
                    return Ok(-1);
            }
            else
                return Ok(-2);
        }

        [HttpGet("NewModule/{moduleId}/{moduleName}")]
        public IActionResult AddModule(string moduleId, string moduleName)
        {
            DbSet<Module> dbs = _dbContext.Module;
            var module = dbs.Where(m => m.ModuleId == moduleId)
                            .FirstOrDefault();

            if (module == null)
            {
                Module newModule = new Module();
                newModule.ModuleId = moduleId;
                newModule.Name = moduleName;

                dbs.Add(newModule);

                DbSet<Lesson> dbsLesson = _dbContext.Lesson;
                for (int i = 1; i <= 13; i++)
                {
                    var newLesson = new Lesson();
                    newLesson.ModuleId = moduleId;
                    newLesson.LessonId = i;
                    newLesson.Notes = "";
                    dbsLesson.Add(newLesson);
                }

                if (_dbContext.SaveChanges() == 14)
                    return Ok(1);
                else
                    return Ok(0);
            }
            else
                return Ok(-1);
        }


        [HttpGet("NewTopic/{moduleId}/{topicId}/{topicTitle}")]
        public IActionResult AddTopic(string moduleId, string topicId, string topicTitle)
        {
            DbSet<Topic> dbs = _dbContext.Topic;
            var topic = dbs.Where(m => m.TopicId == topicId)
                            .FirstOrDefault();

            if (topic == null)
            {
                Topic newTopic = new Topic()
                {
                    ModuleId = moduleId,
                    TopicId = topicId,
                    Title = topicTitle
                };

                dbs.Add(newTopic);

                if (_dbContext.SaveChanges() == 1)
                    return Ok(1);
                else
                    return Ok(0);
            }
            else
                return Ok(-1);
        }

        [HttpGet("{moduleId}/Topics")]
        public IActionResult GetTopicsByModuleId(string moduleId)
        {
            DbSet<Topic> dbs = _dbContext.Topic;
            var topics = dbs.Where(t => t.ModuleId == moduleId)
                            .Select(t => new { t.TopicId, Title = t.Title.Trim() })
                            .ToList();
            return Ok(topics);
        }

        [HttpPost("NewTopic")]
        public IActionResult AddTopic(Topic newTopic)
        {
            DbSet<Topic> dbs = _dbContext.Topic;
            dbs.Add(newTopic);
            if (_dbContext.SaveChanges() == 1)
                return Ok(1);
            else
                return Ok(0);
        }

        [HttpPost("NewAttachment/{moduleId}/{lessonId}")]
        public IActionResult AddAttachment(string moduleId, int lessonId, IFormFile attachment)
        {
            if (attachment != null)
            {
                // TODO Lesson12 Solution Task: Complete AddAttachment action
                // Filename = moduleId_lessondId_attachment.Filename
                // Save to wwwroot\Attachments folder
                string filename = $"{moduleId}_{lessonId}_{attachment.FileName}"; // this line of code needs to be changed
                var filePath = Path.Combine(Environment.CurrentDirectory, $"wwwroot\\Attachments\\{filename}"); // this linle of code needs to be changed

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    attachment.CopyTo(stream);
                }

                DbSet<Lesson> dbs = _dbContext.Lesson;
                var lesson = dbs.Where(l => l.ModuleId == moduleId && l.LessonId == lessonId)
                                .FirstOrDefault();

                if (lesson != null)
                {
                    if (lesson.Attachments == null || lesson.Attachments.Equals(""))
                        lesson.Attachments = filename;
                    else
                        lesson.Attachments += ";" + filename;

                    _dbContext.SaveChanges();
                }
                return Ok(1);
            }
            else
                return Ok(0);
        }

        // TODO Lesson12 Solution Task: Implement GetAttachments action
        // Sample web request http://localhost:62617/api/RPNotes/B215/1/Attachments
        // Sample web response: [{"fileName":"B215_1_18070308538783.pdf","icon":"pdf.png"},{"fileName":"B215_1_SBH[B] - OPTIMA JF.pdf","icon":"pdf.png"},{"fileName":"B215_1_2018 FYP Proposal.docx","icon":"docx.png"}]
        // Hints:
        // 1. Use Split function of string to split the attachments string into string array
        // 2. Use Path.GetExtension to form the icon file name
        [HttpGet("{moduleId}/{lessonId}/Attachments")]
        public IActionResult GetAttachments(string moduleId, int lessonId)
        {
            DbSet<Lesson> dbs = _dbContext.Lesson;
            var attachments = dbs.Where(l => l.ModuleId == moduleId && l.LessonId == lessonId).Select(l => l.Attachments).FirstOrDefault();

            if (attachments != null && !attachments.Equals(""))
            {
                var files = attachments.Split(';').ToList();
                var result = files.Select(f => new
                {
                    fileName = f,
                    icon = $"{Path.GetExtension(f).Remove(0, 1)}.png"
                }).ToList();
                return Ok(result);
            }
            else
                return Ok(0);
        }
    }
}
