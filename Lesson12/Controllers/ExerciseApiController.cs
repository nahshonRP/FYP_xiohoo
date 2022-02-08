using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lesson12.Controllers
{
    [Route("api/Exercise")]
    public class ExerciseApiController : Controller
    {
        [HttpPost]
        public IActionResult Exercise2(IFormFile picture)
        {
            if (picture != null)
            {
                // TODO Lesson12 Task 2-1 Implement part of Exercise2 Web API method which will copy the uploaded file to a physical file
                // Hints:
                // Similar to Task 1-2 except
                // 1. The file should be stored in wwwroot\Exercise2 
                // 2. The file name is based on the FileName property of the IFormFile object (picture)


                var filePath = Path.Combine(Environment.CurrentDirectory, $"wwwroot\\exercise2\\{picture.FileName}");

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    picture.CopyTo(stream);
                }

                return Ok(picture.FileName);


            }
            else
                return Ok(0);
        }
    }
}