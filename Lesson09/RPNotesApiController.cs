using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lesson09.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Lesson09.Controllers
{
    [Route("api/RPNotes")]
    public class RPNotesApiController : Controller
    {
        // TODO Lesson09 Solution Task: Prepare controller for EF dbContext injection
        private AppDbContext _dbContext;
        public RPNotesApiController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // TODO Lesson09 Solution Task: Complete the Get Web Api Action to return the lesson
        // Sample web request: http://localhost:1234/api/RPNotes/C286/3
        // Sample web response: {"moduleId":"C286","moduleName":"Advanced Application Development in .NET","lessonId":3,"notes":"Start on the topic Entity Framework Core"}
        // Note: Return BadRequest() if there is no such lesson
        [HttpGet("{moduleId}/{lessonId}")]
        public IActionResult Get(string moduleId, int lessonId)
        {
            DbSet<Lesson> dbs = _dbContext.Lesson;
            var lesson = dbs.Where(l => l.ModuleId == moduleId
                                     && l.LessonId == lessonId)
                .Include(l => l.Module)
                .Select(l => new { l.ModuleId, ModuleName = l.Module.Name, l.LessonId, l.Notes })
                .FirstOrDefault();

            if (lesson == null)
                return BadRequest();
            else
                return Ok(lesson);
        }

        // TODO Lesson09 Solution Task: Complete the AddModule Web Api Action to return the lesson
        // Sample web request#1: http://localhost:1234/api/RPNotes/NewModule/C105/Introduction to Programming
        // Sample web response#1: 1
        //
        // Sample web request#2: http://localhost:1234/api/RPNotes/NewModule/C286/Advanced Web Application Development in .NET
        // Sample web response#2: -1
        // Notes:
        //  1. Should perform a search to check whether there is such a module first, if there is return -1
        //  2. If there is no such module, then try add the module and also 13 lessons
        //  3. If the addition is successful, which means 14 records were added, return 1
        //  4. If the addition is not successful, return 0
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
    }
}
//19047572 Konada Obadiah Nahshon