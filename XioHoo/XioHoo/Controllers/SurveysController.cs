using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BOL.DBContext;
using CourseMangement.Models;
using CourseMangement.Models.ViewModels;

namespace CourseMangement.Controllers
{
    public class SurveysController : Controller
    {
        private readonly AppDBContext _context;

        public SurveysController(AppDBContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index()
        {
            return View(await _context.Surveys.ToListAsync());
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var survey = await _context.Surveys
                .FirstOrDefaultAsync(m => m.Id == id);
            if (survey == null)
            {
                return NotFound();
            }

            return View(survey);
        }


        public IActionResult Create()
        {
            return View();
        }



        [HttpPost]

        public async Task<IActionResult> Create(Survey survey)
        {
            if (ModelState.IsValid)
            {
                survey.DateCreated = DateTime.Now;
                _context.Add(survey);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(survey);
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var survey = await _context.Surveys.FindAsync(id);
            if (survey == null)
            {
                return NotFound();
            }
            return View(survey);
        }


        [HttpPost]

        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Rating,Comments,Status,DateCreated")] Survey survey)
        {
            if (id != survey.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(survey);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SurveyExists(survey.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(survey);
        }


        private async Task<SurveyQuestionsViewModel> SetAddQUestionsPageState(int id)
        {
            var selectedQuestions = _context.SurveyQuestions.Where(a => a.FkSurveyId == id).ToList();

            var questions = await _context.Questions.Select(a => new QuestionsViewModel { Id = a.Id, Name = a.Name }).ToListAsync();
            foreach (var item in questions)
            {
                if (selectedQuestions.Any(a => a.FkQuestionId == item.Id))
                {
                    item.Selected = true;
                }
            }
            var survey = _context.Surveys.FirstOrDefault(a => a.Id == id);
            return new SurveyQuestionsViewModel
            {
                FkSurveyId = id,
                Questions = questions,
                Survey = survey
            };
        }
        public async Task<IActionResult> AddQuestions(int id)
        {
            var model = await SetAddQUestionsPageState(id);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddQuestions(SurveyQuestionsViewModel model)
        {
            try
            {
                var getquestions = _context.SurveyQuestions.Where(a => a.FkSurveyId == model.FkSurveyId).ToList();
                var onlyselectedquestons = model.Questions.Where(a => a.Selected).ToList();
                if (onlyselectedquestons.Count > 0)
                {
                    _context.SurveyQuestions.RemoveRange(getquestions);
                    foreach (var item in onlyselectedquestons)
                    {
                        _context.SurveyQuestions.Add(new SurveyQuestions { FkQuestionId = item.Id, FkSurveyId = model.FkSurveyId });
                    }
                }

                await _context.SaveChangesAsync();
                ViewData["Message"] = "Questions Successfully Added to this survery";

                var modelRet = await SetAddQUestionsPageState(model.FkSurveyId);
                return View(modelRet);
            }
            catch (Exception ex)
            {
                ViewData["Error"] = ex.Message;
                var modelRet = await SetAddQUestionsPageState(model.FkSurveyId);
                return View(modelRet);
            }
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var survey = await _context.Surveys
                .FirstOrDefaultAsync(m => m.Id == id);
            if (survey == null)
            {
                return NotFound();
            }

            return View(survey);
        }


        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var survey = await _context.Surveys.FindAsync(id);
            _context.Surveys.Remove(survey);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SurveyExists(int id)
        {
            return _context.Surveys.Any(e => e.Id == id);
        }
    }
}
