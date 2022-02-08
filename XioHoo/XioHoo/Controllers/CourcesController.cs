using BOL.DBContext;
using CourseMangement.Models;
using CourseMangement.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseMangement.Controllers
{
    public class CourcesController : Controller
    {
        private readonly AppDBContext dBContext;

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        public CourcesController(AppDBContext _dBContext, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<int>> roleManager)
        {
            dBContext = _dBContext;
            _userManager = userManager;
            _roleManager = roleManager;
        }


        [Authorize]
        // GET: CourcesCotroller
        public ActionResult Index()
        {
            var list = dBContext.Course.Include(a => a.CourseCategories).ToList();
            return View(list);
        }


        private CourseSurveyViewModel SetAssignSurveyState(int id, int surveyid)
        {

            var model = new CourseSurveyViewModel { FkCourceId = id };
            ViewData["Surveys"] = new SelectList(dBContext.Surveys.ToList(), "Id", "Name");
            ViewData["Courses"] = new SelectList(dBContext.Course.ToList(), "Id", "Name", id);
            ViewData["id"] = id;
            var list = dBContext.CourseSurveys.Include(s => s.Course).Include(a => a.Survey)
                .ThenInclude(a => a.SurveyQuestions)
                .FirstOrDefault(a => a.FkCourceId == id);
            if (list != null)
            {
                foreach (var item in list.Survey.SurveyQuestions)
                {
                    var questions = dBContext.Questions.
                            Where(a => a.Id == item.FkQuestionId).FirstOrDefault();
                    item.Question = questions;
                }
                ViewData["Surveys"] = new SelectList(dBContext.Surveys.ToList(), "Id", "Name", list.FkSurveyId);
                model.Id = list.Id;
                model.Survey = list.Survey;
                model.Course = list.Course;
                model.FkSurveyId = list.FkSurveyId;
                return model;
            }

            if (surveyid == 0)
            {
                ViewData["Surveys"] = new SelectList(dBContext.Surveys.ToList(), "Id", "Name");
            }
            else
                ViewData["Surveys"] = new SelectList(dBContext.Surveys.ToList(), "Id", "Name", surveyid);

            if (list == null)
            {
                var course = dBContext.Course.Find(id);
                model.Course = course;
                if (surveyid != 0)
                {
                    var surveyrec = dBContext.Surveys
                  .Include(a => a.SurveyQuestions)
                  .FirstOrDefault(a => a.Id == surveyid);

                    var surveyquestionids = surveyrec.SurveyQuestions.Select(a => a.FkQuestionId).Distinct().ToList();
                    foreach (var item in surveyrec.SurveyQuestions)
                    {
                        var questions = dBContext.Questions.
                            Where(a => a.Id == item.FkQuestionId).FirstOrDefault();
                        item.Question = questions;
                    }

                    model.Survey = surveyrec;
                }
            }
            return model;
        }


        private async Task<CourseSurveyViewModel> SetFillSurveyState(int id)
        {
            var model = new CourseSurveyViewModel { FkCourceId = id };
            var list = dBContext.CourseSurveys.Include(s => s.Course).Include(a => a.Survey)
                .ThenInclude(a => a.SurveyQuestions)
                .FirstOrDefault(a => a.FkCourceId == id);
            if (list != null)
            {
                var user = await _userManager.GetUserAsync(User);
                var userid = 0;
                if (user != null)
                {
                    userid = user.Id;
                }
                var submittedsuvey = dBContext.UsersSurveys.Include(a => a.UsersSurveyDetails).FirstOrDefault(a => a.FkUserId == userid && a.FkCourseSurveyId == list.Id);

                var listquestions = new List<QuestionsViewModel>();
                foreach (var item in list.Survey.SurveyQuestions)
                {
                    var questions = dBContext.Questions.
                            Where(a => a.Id == item.FkQuestionId).FirstOrDefault();
                    item.Question = questions;
                    var selectedq = submittedsuvey == null ? null : submittedsuvey.UsersSurveyDetails.FirstOrDefault(d => d.FkQuestionID == item.FkQuestionId);
                    var val = selectedq == null ? "" : selectedq.AnswerScore.ToString();
                    listquestions.Add(new QuestionsViewModel { Id = questions.Id, Name = questions.Name, Selected = true, Value = val });
                }
                ViewData["Surveys"] = new SelectList(dBContext.Surveys.ToList(), "Id", "Name", list.FkSurveyId);
                model.Id = list.Id;
                model.Survey = list.Survey;
                model.Course = list.Course;
                model.FkSurveyId = list.FkSurveyId;
                model.Questions = listquestions;


                model.IsAlreadySubnitted = submittedsuvey != null;
                return model;
            }
            else
            {
                var course = dBContext.Course.Find(id);
                model.Course = course;
                //model.Survey = surveyrec;
            }
            return model;
        }

        [Authorize]
        // GET: CourcesCotroller
        [HttpGet]
        public async Task<IActionResult> FillSurvey(int id)
        {
            var model = await SetFillSurveyState(id);
            return View(model);
        }


        [Authorize]

        [HttpPost]
        public async Task<IActionResult> FillSurvey(int id, CourseSurveyViewModel model)
        {

            try
            {
                var list = dBContext.CourseSurveys.Include(s => s.Course).Include(a => a.Survey)
                       .ThenInclude(a => a.SurveyQuestions)
                       .FirstOrDefault(a => a.FkCourceId == id);
                if (list != null)
                {
                    var user = await _userManager.GetUserAsync(User);
                    var userid = 0;
                    if (user != null)
                    {
                        userid = user.Id;
                    }

                    var totalquestions = model.Questions.Count;
                    var totalscore = totalquestions * 5;
                    var score = model.Questions.Sum(a => Convert.ToInt32(a.Value));
                    var average = score * 5;

                    var usersurvey = new UsersSurvey
                    {
                        FkUserId = userid,
                        DateAdded = DateTime.Now,
                        Comments = model.Comment,
                        Ratings = (int)average,
                        FkCourseSurveyId = list.Id,

                    };
                    dBContext.UsersSurveys.Add(usersurvey);
                    dBContext.SaveChanges();
                    foreach (var item in model.Questions)
                    {
                        dBContext.UsersSurveyDetails.Add(new UsersSurveyDetails
                        {
                            AnswerScore = Convert.ToInt32(item.Value),
                            DateAdded = DateTime.Now,
                            FkQuestionID = item.Id,
                            FkUserSruveyID = usersurvey.Id,
                            FkQuestionText = item.Name
                        });
                        dBContext.SaveChanges();
                    }

                }
                var vmodel = await SetFillSurveyState(id);
                ViewData["Message"] = "Survey Successfully submitted!";
                return View(vmodel);
            }
            catch (Exception ex)
            {
                ViewData["Error"] = ex.Message;
                var vmodel = await SetFillSurveyState(id);
                return View(vmodel);
            }
        }
        [Authorize]

        [HttpGet]
        public ActionResult AssignSurvey(int id, int surveyid = 0)
        {
            var model = new CourseSurveyViewModel { FkCourceId = id, FkSurveyId = surveyid };
            ViewData["Courses"] = new SelectList(dBContext.Course.ToList(), "Id", "Name", id);
            ViewData["id"] = id;
            var list = dBContext.CourseSurveys.Include(s => s.Course).Include(a => a.Survey)
                .ThenInclude(a => a.SurveyQuestions)
                .FirstOrDefault(a => a.FkCourceId == id);
            if (list != null)
            {
                foreach (var item in list.Survey.SurveyQuestions)
                {
                    var questions = dBContext.Questions.
                            Where(a => a.Id == item.FkQuestionId).FirstOrDefault();
                    item.Question = questions;
                }
                ViewData["Surveys"] = new SelectList(dBContext.Surveys.ToList(), "Id", "Name", list.FkSurveyId);
                model.Id = list.Id;
                model.Survey = list.Survey;
                model.Course = list.Course;
                model.FkSurveyId = list.FkSurveyId;
                return View(model);
            }

            if (surveyid == 0)
            {
                ViewData["Surveys"] = new SelectList(dBContext.Surveys.ToList(), "Id", "Name");
            }
            else
                ViewData["Surveys"] = new SelectList(dBContext.Surveys.ToList(), "Id", "Name", surveyid);

            if (list == null)
            {
                var course = dBContext.Course.Find(id);
                model.Course = course;
                if (surveyid != 0)
                {
                    var surveyrec = dBContext.Surveys
                  .Include(a => a.SurveyQuestions)
                  .FirstOrDefault(a => a.Id == surveyid);

                    var surveyquestionids = surveyrec.SurveyQuestions.Select(a => a.FkQuestionId).Distinct().ToList();
                    foreach (var item in surveyrec.SurveyQuestions)
                    {
                        var questions = dBContext.Questions.
                            Where(a => a.Id == item.FkQuestionId).FirstOrDefault();
                        item.Question = questions;
                    }

                    model.Survey = surveyrec;
                }


            }
            return View(model);
        }
        [Authorize]

        public ActionResult AssignSurvey(CourseSurveyViewModel model)
        {
            try
            {
                var list = dBContext.CourseSurveys.FirstOrDefault(a => a.FkCourceId == model.FkCourceId);
                var state = SetAssignSurveyState(model.FkCourceId, model.FkSurveyId);
                state.FkSurveyId = model.FkSurveyId;

                if (list != null)
                {

                    var coursesurvey = dBContext.CourseSurveys.Find(model.FkCourceId);
                    coursesurvey.FkSurveyId = model.FkSurveyId;
                    dBContext.Update(coursesurvey);
                    dBContext.SaveChanges();

                    ViewData["Message"] = "Course is Attached to a  Survey Successfully";
                    return View(state);
                }
                var coursemodel = new CourseSurvey { FkCourceId = model.FkCourceId, FkSurveyId = model.FkSurveyId };
                dBContext.CourseSurveys.Add(coursemodel);
                dBContext.SaveChanges();
                ViewData["Message"] = "Course is Attached to a  Survey Successfully";
                state.Id = coursemodel.Id;
                return View(state);
            }
            catch (Exception ex)
            {
                var state = SetAssignSurveyState(model.FkCourceId, model.FkSurveyId);
                state.FkSurveyId = model.FkSurveyId;
                ViewData["Error"] = ex.Message;
                return View(state);
            }
        }

        [Authorize]
        public async Task<IActionResult> Participate(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var userid = 0;
            if (user != null)
            {
                userid = user.Id;
                var alredyadded = dBContext.CourseAssignees.Any(a => a.fkCourseId == id && a.fkUserId == userid);
                if (alredyadded)
                {
                    return RedirectToAction("Browse", new { message = "already" });
                }

                dBContext.CourseAssignees.Add(new CourseAssignee { fkCourseId = id, fkUserId = userid });
                dBContext.SaveChanges();
                ViewData["id"] = id;
                return RedirectToAction("Browse", new { message = "sucess" });
            }
            else
            {
                return RedirectToAction("Browse", new { message = "nouser" });
            }

        }


        [Authorize]
        public async Task<IActionResult> RemoveUser(int id, int userId)
        {


            var alredyadded = await dBContext.CourseAssignees.FirstOrDefaultAsync(a => a.fkCourseId == id && a.fkUserId == userId);
            if (alredyadded == null)
            {
                return RedirectToAction("index", new { message = "not" });
            }

            dBContext.CourseAssignees.Remove(alredyadded);
            dBContext.SaveChanges();
            ViewData["id"] = id;
            return RedirectToAction("AssignToUser", new { id = id, message = "sucess" });


        }


        public ActionResult Browse(string message = "")
        {
            if (!string.IsNullOrEmpty(message))
            {
                if (message == "sucess")
                {
                    ViewData["Message"] = "Course is Assigned to you  Successfully";
                }
                else if (message == "nouser")
                {
                    ViewData["Error"] = "Please Login/Regiter First";
                }
                else
                {
                    ViewData["Error"] = "Course Already assigned to you";
                }
            }
            var list = dBContext.Course.Include(a => a.CourseCategories).ToList();
            return View(list);
        }


        [Authorize]

        public async Task<IActionResult> Assigned(int id)
        {
            var userid = 0;
            if (id > 0)
            {
                userid = id;
            }
            else
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    userid = user.Id;

                }
                else
                {
                    return View(new List<Course>());
                }
            }
            var courceIds = dBContext.CourseAssignees.Where(a => a.fkUserId == userid).Select(a => a.fkCourseId).ToList();
            var cources = await dBContext.Course.Include(a => a.CourseCategories).Where(a => courceIds.Contains(a.Id)).ToListAsync();
            return View(cources);
        }


        [Authorize(Roles = "TRAINER,ADMIN")]
        public async Task<IActionResult> Attendance(int id)
        {
            var model = await AttendancePageStates(id);
            return View(model);
        }

        private async Task<CourseAttendanceViewModel> AttendancePageStates(int id)
        {
            var userids = await dBContext.CourseAssignees.Where(a => a.fkCourseId == 1).Select(a => a.fkUserId).ToListAsync();
            var users = dBContext.Users.Where(a => userids.Contains(a.Id) && a.RoleName.ToLower() != "trainer").ToList();
            var course = dBContext.Course.Find(id);


            var model = new CourseAttendanceViewModel
            {
                AttendaceDateTime = DateTime.Now,
                Course = course,
                FkCourseId = id,
                FkUserId = 0,
                Users = users
            };
            return model;
        }

        [Authorize(Roles = "TRAINER")]
        [HttpPost]
        public async Task<IActionResult> Attendance(int id, CourseAttendanceViewModel model)
        {
            CourseAttendanceViewModel viewmodel = null;
            var cc = await dBContext.Users.ToListAsync();
            return View(viewmodel);
        }



        public ActionResult AssignToUser(int id)
        {
            var course = dBContext.Course.Include(a => a.CourseCategories).FirstOrDefault(a => a.Id == id);
            var assignedPrticIds = dBContext.CourseAssignees.Where(a => a.fkCourseId == id).Select(a => a.fkUserId).ToList();
            var assignedusers = dBContext.Users.Where(a => assignedPrticIds.Contains(a.Id)).ToList();
            var unassigned = dBContext.Users.Where(a => !assignedPrticIds.Contains(a.Id) && (a.RoleName.Contains("TRAINER") || a.RoleName.Contains("PARTICI"))).ToList();
            ViewData["id"] = id;
            ViewData["UnassignedUserArr"] = unassigned;
            ViewData["AssignedUsers"] = assignedusers;
            ViewData["Courses"] = new SelectList(dBContext.Course.ToList(), "Id", "Name", course.Id);
            ViewData["Users"] = new SelectList(unassigned, "Id", "FullName", course.Id);
            return View();
        }

        [HttpPost]
        public ActionResult AssignToUser(int id, CourseAssignee model)
        {
            try
            {
                dBContext.CourseAssignees.Add(new CourseAssignee { fkCourseId = model.fkCourseId, fkUserId = model.fkUserId });
                dBContext.SaveChanges();
                ViewData["id"] = id;
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                var course = dBContext.Course.Include(a => a.CourseCategories).FirstOrDefault(a => a.Id == id);
                ViewData["id"] = id;
                ViewData["Course"] = course;
                ViewData["Courses"] = new SelectList(dBContext.Course.ToList(), "Id", "Name", course.Id);
                ViewData["Users"] = new SelectList(dBContext.Users.Where(a => a.RoleName.Contains("TRAINER") || a.RoleName.Contains("PARTICI")).ToList(), "Id", "FullName", course.Id);
                return View(model);
            }
        }

        public ActionResult Details(int id)
        {
            var course = dBContext.Course.Include(a => a.CourseCategories).FirstOrDefault(a => a.Id == id);
            ViewData["FkCourseCategoryId"] = new SelectList(dBContext.CourseCategories.ToList(), "Id", "Name", course.FkCourseCategoryId);
            return View(course);
        }


        public ActionResult Create()
        {
            ViewData["FkCourseCategoryId"] = new SelectList(dBContext.CourseCategories.ToList(), "Id", "Name");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Course model)
        {
            try
            {
                dBContext.Course.Add(model);
                dBContext.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ViewData["FkCourseCategoryId"] = new SelectList(dBContext.CourseCategories.ToList(), "Id", "Name", model.FkCourseCategoryId);
                return View(model);
            }
        }


        public ActionResult Edit(int id)
        {
            var course = dBContext.Course.Find(id);
            ViewData["FkCourseCategoryId"] = new SelectList(dBContext.CourseCategories.ToList(), "Id", "Name", course.FkCourseCategoryId);
            return View(course);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Course model)
        {
            try
            {
                dBContext.Course.Update(model);
                dBContext.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ViewData["FkCourseCategoryId"] = new SelectList(dBContext.CourseCategories.ToList(), "Id", "Name", model.FkCourseCategoryId);
                return View(model);
            }
        }


        public ActionResult Delete(int id)
        {
            var course = dBContext.Course.Include(a => a.CourseCategories).FirstOrDefault(a => a.Id == id);
            ViewData["FkCourseCategoryId"] = new SelectList(dBContext.CourseCategories.ToList(), "Id", "Name", course.FkCourseCategoryId);
            return View(course);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Course model)
        {
            try
            {
                dBContext.Course.Remove(model);
                dBContext.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(model);
            }
        }
    }
}
