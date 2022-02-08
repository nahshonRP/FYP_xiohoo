using BOL.DBContext;
using CourseMangement.Models;
using CourseMangement.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CourseMangement.Controllers
{
    public class AccountController : Controller
    {

        private readonly AppDBContext dBContext;
        private readonly SignInManager<ApplicationUser> _signinManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        public AccountController(AppDBContext _dBContext, SignInManager<ApplicationUser> signinManager, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<int>> roleManager)
        {
            dBContext = _dBContext;
            _signinManager = signinManager;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> SignOut()
        {
            await _signinManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        public IActionResult Login(string returnUrl = "")
        {
            ViewData["returnUrl"] = returnUrl;
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Login(UserLogin model, string returnUrl = "")
        {
            var isPersistent = true;

            var usere = await _userManager.FindByNameAsync(model.UserName);
            if (usere != null)
            {
                if (!usere.UserStatus)
                {
                    ViewData["Errors"] = "User is currently deactivated!";
                    return View(model);
                }
                var aa = await _signinManager.CheckPasswordSignInAsync(usere, model.Password, isPersistent);
                if (aa.Succeeded)
                {
                    var customClaims = new[]
                                  {
                                            new Claim(ClaimTypes.NameIdentifier, usere.Id.ToString()),
                                            new Claim(ClaimTypes.Name, usere.FullName),
                                            new Claim(ClaimTypes.Role, usere.RoleName),
                                            new Claim("UserName", model.UserName),
                                 };
                    var claimsPrincipal = await _signinManager.CreateUserPrincipalAsync(usere);
                    if (customClaims != null && claimsPrincipal?.Identity is ClaimsIdentity claimsIdentity)
                    {
                        claimsIdentity.AddClaims(customClaims);
                    }

                    await _signinManager.Context.SignInAsync(IdentityConstants.ApplicationScheme,
                        claimsPrincipal,
                        new AuthenticationProperties { IsPersistent = isPersistent });

                    if (!String.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        return Redirect(returnUrl);

                    if (await _userManager.IsInRoleAsync(usere, "ADMIN"))
                    {
                        return RedirectToAction("Index", "Users");
                    }
                    else
                    {
                        return RedirectToAction("Profile", "users", new { id = usere.Id });
                    }

                }
                ViewData["Errors"] = "Invalid username or password";
            }
            ViewData["Errors"] = "User not found";
            return View(model);
        }

        public IActionResult SignUp(string returnUrl = "")
        {
            var user = new ApplicationUser { PhoneNumber = returnUrl };
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SignUp(ApplicationUser model)
        {
            try
            {
                model.Joiningdate = DateTime.Now;
                model.RoleName = "PARTICIPANT";
                model.UserStatus = true;
                var redirecturl = model.PhoneNumber;
                model.PhoneNumber = "";
                var res = await _userManager.CreateAsync(model, model.PlainPassword);
                if (res.Succeeded)
                {
                    var roleRes = await _userManager.AddToRoleAsync(model, model.RoleName);

                    var aa = await _signinManager.CheckPasswordSignInAsync(model, model.PlainPassword, true);
                    if (aa.Succeeded)
                    {
                        var customClaims = new[]
                               {
                                            new Claim(ClaimTypes.NameIdentifier, model.Id.ToString()),
                                            new Claim(ClaimTypes.Name, model.FullName),
                                            new Claim(ClaimTypes.Role, model.RoleName),
                                            new Claim("UserName", model.UserName),
                                 };
                        var claimsPrincipal = await _signinManager.CreateUserPrincipalAsync(model);
                        if (customClaims != null && claimsPrincipal?.Identity is ClaimsIdentity claimsIdentity)
                        {
                            claimsIdentity.AddClaims(customClaims);
                        }

                        await _signinManager.Context.SignInAsync(IdentityConstants.ApplicationScheme,
                            claimsPrincipal,
                            new AuthenticationProperties { IsPersistent = true });
                    }
                }
                else
                {
                    ViewData["Error"] = "Us";
                    return View(model);
                }




                if (!String.IsNullOrEmpty(redirecturl) && Url.IsLocalUrl(redirecturl))
                {
                    return Redirect(redirecturl);
                }

                return RedirectToAction("Profile", "users", new { id = model.Id });
            }
            catch
            {
                // SetPageData();
                return View(model);
            }
        }

    }
}
