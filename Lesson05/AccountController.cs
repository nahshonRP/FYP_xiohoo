using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Data;
using System.Security.Claims;
using Lesson05.Models;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Lesson05.Controllers
{

    public class AccountController : Controller
    {
        private const string AUTHSCHEME = "UserSecurity";
        private const string LOGIN_SQL =
           @"SELECT * FROM AppUser 
            WHERE Id = '{0}' 
              AND Password = HASHBYTES('SHA1', '{1}')";

        private const string LASTLOGIN_SQL =
           @"UPDATE AppUser SET LastLogin=GETDATE() 
                        WHERE Id='{0}'";

        private const string ROLE_COL = "Role";
        private const string NAME_COL = "Name";

        private const string REDIRECT_CNTR = "Home";
        private const string REDIRECT_ACTN = "Index";

        private const string LOGIN_VIEW = "Login";

        private AppDbContext _dbContext;

        public AccountController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            TempData["ReturnUrl"] = returnUrl;
            return View(LOGIN_VIEW);
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login(LoginUser user)
        {
            if (!AuthenticateUser(user.UserId, user.Password, out ClaimsPrincipal principal))
            {
                ViewData["Message"] = "Incorrect User Id or Password";
                ViewData["MsgType"] = "warning";
                return View(LOGIN_VIEW);
            }
            else
            {
                HttpContext.SignInAsync(
                   AUTHSCHEME,
                   principal,
               new AuthenticationProperties
               {
                   IsPersistent = false
               });

                // Update the Last Login Timestamp of the User

                // TODO Lesson05 Task 1-2 Use DbContext.Database.ExecuteSqlRaw method to update the last login date of the current user
                string sql = String.Format(LASTLOGIN_SQL, user.UserId);
                int num_affected = _dbContext.Database.ExecuteSqlRaw(sql); // modify this line of code
                // TODO Lesson05 Task 1-4 Use DbContext.Database.ExecuteSqlInterpolated method to rewrite the code in Task 1-3
                int num_record = _dbContext.Database.ExecuteSqlInterpolated($@"UPDATE AppUser SET LastLogin=GETDATE() Where Id = {user.UserId}");
                if (TempData["returnUrl"] != null)
                {
                    string returnUrl = TempData["returnUrl"].ToString();
                    if (Url.IsLocalUrl(returnUrl))
                        return Redirect(returnUrl);
                }

                return RedirectToAction(REDIRECT_ACTN, REDIRECT_CNTR);
            }
        }

        [Authorize]
        public IActionResult Logoff(string returnUrl = null)
        {
            HttpContext.SignOutAsync(AUTHSCHEME);
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            return RedirectToAction(REDIRECT_ACTN, REDIRECT_CNTR);
        }

        [AllowAnonymous]
        public IActionResult Forbidden()
        {
            return View();
        }

        private bool AuthenticateUser(string uid, string pw, out ClaimsPrincipal principal)
        {
            // TODO Lesson05 Task 1-1 Use DbSet.FromSqlRaw method to retrieve the appUser:
            DbSet<AppUser> dbs = _dbContext.AppUser;

            string sql = String.Format(LOGIN_SQL, uid, pw);
            AppUser appUser = dbs.FromSqlRaw(sql).FirstOrDefault(); // This code needs to be modified

            // TODO Lesson05 Task 1-3 Use DbSet.FromSqlInterpolated method to rewrite the code in Task 1-1

            AppUser appUser2 = dbs.FromSqlInterpolated($@"SELECT * FROM AppUser WHERE ID = {uid} AND Password = HASHBYTES('SHA1', CONVERT(VARCHAR, {pw}))").FirstOrDefault();

            principal = null;

            //DataTable ds = DBUtl.GetTable(LOGIN_SQL, uid, pw);

            if (appUser != null)
            {
                principal =
                   new ClaimsPrincipal(
                      new ClaimsIdentity(
                         new Claim[] {
                        new Claim(ClaimTypes.NameIdentifier, appUser.Id),
                        new Claim(ClaimTypes.Name, appUser.Name),
                        new Claim(ClaimTypes.Role, appUser.Role)
                         }, "Basic"
                      )
                   );
                return true;
            }
            return false;
        }

        [Authorize]
        public JsonResult VerifyCurrentPassword(string CurrentPassword)
        {
            DbSet<AppUser> dbs = _dbContext.AppUser;
            var userid = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            // TODO Lesson05 Task Solution - Use FromSqlInterpolated to retrieve AppUser with userid and password
            AppUser user = dbs.FromSqlInterpolated($@"SELECT * FROM AppUser WHERE Id= {userid} AND Password = HASHBYTES('SHA1', CONVERT(VARCHAR, {CurrentPassword}))").FirstOrDefault();

            if (user != null)
                return Json(true);
            else
                return Json(false);
        }

        [Authorize]
        public JsonResult VerifyNewPassword(string NewPassword)
        {
            // TODO Lesson05 Task Solution - Similar to VerifyCurrentPassword but return true and false in reverse condition

            DbSet<AppUser> dbs = _dbContext.AppUser;
            var userid = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            AppUser user = dbs.FromSqlInterpolated($@"SELECT * FROM AppUser WHERE Id= {userid} AND Password = HASHBYTES('SHA1', CONVERT(VARCHAR, {NewPassword}))").FirstOrDefault();



            if (user != null)
                return Json(false);
            else
                return Json(true);
        }

        // TODO Lesson05 Task Solution - implement ChangePassword HttpGet and HttpPost here

        [Authorize]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult ChangePassword(PasswordUpdate pu)
        {
            var userid = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var sql = String.Format($"UPDATE AppUser SET Password = HASHBYTES ('SHA1', '{pu.NewPassword}') " +
                $"WHERE Id = '{userid}' AND Password = HASHBYTES ('SHA1', '{pu.CurrentPassword}')");

            if (_dbContext.Database.ExecuteSqlRaw(sql) == 1)
            {
                ViewData["Msg"] = "password has been updated!";
            }
            else
            {
                ViewData["Msg"] = "failed to update password!";
            }
            return View();
        }

    }
}
//19047572 Konada Obadiah Nahshon