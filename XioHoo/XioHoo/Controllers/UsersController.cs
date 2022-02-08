using BOL.DBContext;
using CourseMangement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseMangement.Controllers
{
    public class UsersController : Controller
    {
        private readonly AppDBContext dBContext;

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        public UsersController(AppDBContext _dBContext, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<int>> roleManager)
        {
            dBContext = _dBContext;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [Authorize]
        // GET: UsersController
        public ActionResult Index()
        {
            var users = dBContext.Users.ToList();

            return View(users);
        }



        public async Task<ActionResult> Profile(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            SetPageData();
            return View(user);
        }

        [HttpPost]
        public async Task<ActionResult> Profile(int id, ApplicationUser model)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id.ToString());
                user.FullName = model.FullName;
                user.DOB = model.DOB;

                if (user.PlainPassword != model.PlainPassword)
                {
                    var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                    //HttpContext.Session.Set<UserInfo>("user", model);
                    var result = await _userManager.ResetPasswordAsync(user, code, model.PlainPassword);
                    if (result.Succeeded)
                    {
                        user.PlainPassword = model.PlainPassword;
                    }
                }
                dBContext.SaveChanges();
                ViewData["Message"] = "Questions Successfully Added to this survery";

            }
            catch (Exception ex)
            {
                ViewData["Error"] = ex.Message;
                SetPageData();
            }
            return View(model);

        }


        public async Task<ActionResult> Details(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            SetPageData();
            return View(user);
        }


        public ActionResult Create()
        {
            SetPageData();
            return View();
        }

        private void SetPageData()
        {
            ViewData["RoleName"] = new SelectList(dBContext.Roles.ToList(), "Name", "Name");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ApplicationUser model)
        {
            try
            {
                SetPageData();
                model.Joiningdate = DateTime.Now;
                var aa = await _userManager.CreateAsync(model, model.PlainPassword);
                if (aa.Succeeded)
                {
                    var roleRes = await _userManager.AddToRoleAsync(model, model.RoleName);
                    return RedirectToAction(nameof(Index));
                }

                ViewData["Error"] = "User Registration failed";
                return View(model);
            }
            catch
            {
                SetPageData();
                return View(model);
            }
        }


        public async Task<ActionResult> Edit(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            SetPageData();

            return View(user);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, ApplicationUser model)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id.ToString());
                user.FullName = model.FullName;
                user.DOB = model.DOB;
                user.UserStatus = model.UserStatus;

                if (user.PlainPassword != model.PlainPassword)
                {
                    var code = await _userManager.GeneratePasswordResetTokenAsync(user);

                    var result = await _userManager.ResetPasswordAsync(user, code, model.PlainPassword);
                    if (result.Succeeded)
                    {
                        user.PlainPassword = model.PlainPassword;
                    }
                }
                if (user.RoleName != model.RoleName)
                {
                    var res = await _userManager.RemoveFromRoleAsync(user, user.RoleName);
                    var roleRes = await _userManager.AddToRoleAsync(user, model.RoleName);
                    user.RoleName = model.RoleName;
                }
                dBContext.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                SetPageData();
                return View(model);
            }
        }


        public async Task<ActionResult> Delete(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            SetPageData();
            return View(user);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, IFormCollection collection)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id.ToString());
                await _userManager.DeleteAsync(user);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                SetPageData();
                return View();
            }
        }
    }
}
