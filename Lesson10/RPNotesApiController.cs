using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lesson10.Models;


namespace Lesson10.Controllers
{
    [Route("api/RPNotes")]
    public class RPNotesApiController : Controller
    {
        private AppDbContext _dbContext;

        public RPNotesApiController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // TODO P10 Solution Task: Implement GetAllModules action which return all modules as a list in ascending order by module id
        // Sample web request: http://localhost:1234/api/RPNotes/Modules
        // Sample web response: 
        //  [{"moduleId":"B215","name":"Financial Accounting","lesson":[],"topic":[]},
        //   {"moduleId":"C286","name":"Advanced Application Development in .NET","lesson":[],"topic":[]}]
        [HttpGet("Modules")]
        public IActionResult GetAllModules()
        {
            DbSet<Module> dbs = _dbContext.Module;
            var modules = dbs.OrderBy(m => m.ModuleId).ToList();

            return Ok(modules);
        }

        // TODO P10 Solution Task: Implement UpdateNotes action which accept a Lesson object and update to database
        // Return values:
        //  If invalid data detected, return -2
        //  If target lesson not found, return -1
        //  If update database failed, return 0
        //  If update database successful, return 1
        [HttpPost("Save")]
        public IActionResult UpdateNotes(Lesson ulesson)
        {
            if (ModelState.IsValid)
            {
                DbSet<Lesson> dbs = _dbContext.Lesson;
                var tLesson = dbs.Where(l => l.ModuleId == ulesson.ModuleId && l.LessonId == ulesson.LessonId).FirstOrDefault();

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

        // TODO P10 Solution Task: Implement AddTopic action which accept three strings namely moduleId, topicId and topicTitle and insert a new topic
        // Return values:
        //  If topic already exists, return -1
        //  If insert to database filed, return 0
        //  If insert to database successful, return 1
        [HttpGet("NewTopic/{moduleId}/{topicId}/{topicTitle}")]
        public IActionResult AddTopic(string moduleId, string topicId, string topicTitle)
        {
            DbSet<Topic> dbs = _dbContext.Topic;
            var topic = dbs.Where(m => m.TopicId == topicId).FirstOrDefault();

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
    }
}
//19047572 Konada Obadiah Nahshon
