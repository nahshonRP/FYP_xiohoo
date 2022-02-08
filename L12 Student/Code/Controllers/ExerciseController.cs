using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lesson12.Controllers
{
    public class ExerciseController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Exercise1()
        {
            return View();
        }

        // TODO Lesson12 Task 1-3 Implement Exercise1 to allow picture upload
        // Use 6P Part1 slides to help you
        [HttpPost]
        public IActionResult Exercise1(IFormFile picture)
        {
            // Implement your code here
            return View();
        }

        public IActionResult Exercise2()
        {
            return View();
        }

        public IActionResult Exercise3()
        {
            return View();
        }

        public IActionResult Exercise4()
        {
            return View();
        }
    }
}