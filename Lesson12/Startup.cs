using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lesson12
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddControllers().AddNewtonsoftJson();
            services.AddDbContext<Lesson12.Models.AppDbContext>(
                options =>
                    options.UseSqlServer(
                        Configuration.GetConnectionString("DefaultConnection")));

        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(routes =>
            {
                routes.MapControllerRoute(
                         name: "ViewByModuleIdLessonId",
                         pattern: "{moduleId}/{frLessonId:int}/{toLessonId:int?}",
                         defaults: new { controller = "RPNotes", action = "ListByModuleLesson" },
                         constraints: new { moduleId = @"[A-Za-z]\d{3}" });

                routes.MapControllerRoute(
                    name: "ViewByModuleId",
                    pattern: "{moduleId}",
                    defaults: new { controller = "RPNotes", action = "ListByModule" },
                    constraints: new { moduleId = @"[A-Za-z]\d{3}" });

                routes.MapControllerRoute(
                    name: "ViewByTopics",
                    pattern: "topic",
                    defaults: new { controller = "RPNotes", action = "TopicalIndex" });

                routes.MapControllerRoute(
                name: "Search",
                    pattern: ":{keyPhrase}",
                    defaults: new { controller = "RPNotes", action = "Search" });

                routes.MapControllerRoute(
                         name: "rpNotesdefault",
                         pattern: "{controller=RPNotes}/{action=Index}/{id?}");

                routes.MapControllerRoute(
                 name: "default",
                 pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
