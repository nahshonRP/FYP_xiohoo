using BOL.DBContext;
using CourseMangement.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseMangement
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddDbContext<AppDBContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<AppDBContext>(options =>
                  options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            //services.AddIdentityCore<ApplicationUser>()
            //    .AddRoles<IdentityRole<int>>()
            //    .AddEntityFrameworkStores<AppDBContext>();

            services.AddIdentity<ApplicationUser, IdentityRole<int>>()
            .AddEntityFrameworkStores<AppDBContext>()
            //.AddDefaultUI()
            .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                options.Cookie.Name = "CourseM";
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                options.LoginPath = "/Account/Login";
                // ReturnUrlParameter requires 
                //using Microsoft.AspNetCore.Authentication.Cookies;
                options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
                options.SlidingExpiration = true;
            });

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                //options.Password.RequiredLength = 6;
                //options.Password.RequiredUniqueChars = 1;


                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = false;
            });
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme) //Default Auth-Scheme
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.Cookie.Name = "CourseM";
                options.LoginPath = "/Account/Login";
            });

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider services)
        {

            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var RoleManager = services.GetRequiredService<RoleManager<IdentityRole<int>>>();
                CreateRoles(services).Wait();
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private async Task CreateRoles(IServiceProvider serviceProvider)
        {
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            string[] roleNames = { "ADMIN", "TRAINER", "PARTICIPANT" };
            IdentityResult roleResult;
            //   "Electrition","Painter","Cleaner", "Home Shifting","Laundry"
            //Role area
            foreach (var roleName in roleNames)
            {
                var roleExist = await RoleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    //create the roles and seed them to the database: Question 1  
                    roleResult = await RoleManager.CreateAsync(new IdentityRole<int>(roleName));
                }
            }

            //user creation area
            ApplicationUser user = await UserManager.FindByEmailAsync("admin@gmail.com");

            if (user == null)
            {
                user = new ApplicationUser()
                {
                    FullName = "Admin Name",
                    UserName = "admin@gmail.com",
                    Email = "admin@gmail.com",
                    RoleName = "ADMIN",
                    UserStatus = true
                };
                await UserManager.CreateAsync(user, "Test@123");
                await UserManager.AddToRoleAsync(user, "ADMIN");

            }

            ApplicationUser user2 = await UserManager.FindByEmailAsync("participant@gmail.com");

            if (user2 == null)
            {
                user2 = new ApplicationUser()
                {
                    FullName = "PARTICIPANT 1",
                    UserName = "PARTICIPANT@gmail.com".ToLower(),
                    Email = "PARTICIPANT@gmail.com".ToLower(),
                    RoleName = "PARTICIPANT",
                    UserStatus = true
                };
                await UserManager.CreateAsync(user2, "Test@123");
                await UserManager.AddToRoleAsync(user2, "PARTICIPANT");

            }

        }

    }
}
