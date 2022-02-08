using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lesson11
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
            services.AddDbContext<Lesson11.Models.AppDbContext>(
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

                //// TODO L11 Task 2-1: Custom Routing Exercise 5
                //// Routing example: http://localhost/Shoppers/0L8013 will display order details for order id 0l8013
                //// Note: All order id starts with 1 digit, followed by 1 uppercase alphabet ranging from A to L, 
                //// and followed by 4 digits
                routes.MapControllerRoute(
                name: "ShoppersByOrderId",
                   pattern: "Shoppers/{id}",
                   defaults: new { controller = "Order", action = "GetOrder" },
                   constraints: new { id = @"\d[A-L]\d{4}" });

                //// TODO L11 Task 2-1: Custom Routing Exercise 4
                //// Routing example: http://localhost/Shoppers/MO051 will display product details for product id MO051
                //// Note: All product id starts with 2 upper case alphabets and followed by 3 digits
                routes.MapControllerRoute(
                    name: "ShoppersByProductId",
                    pattern: "Shoppers/{id}",
                    defaults: new { controller = "Product", action = "GetProduct" },
                    constraints: new { id = @"[A-Z]{2}\d{3}" });

                //// TODO L11 Task 2-1: Custom Routing Exercise 3
                //// Routing example: http://localhost/Shoppers/2018/01/01 will display all orders with order date 2017/01/01
                //// Note: oyear must be 4 digits, omonth must be 2 digits, oday must be 2 digits
                ////      all variables (oyear, omonth and oday) must be of type integer
                routes.MapControllerRoute(
                    name: "ShoppersByOrderDate",
                    pattern: "Shoppers/{oyear:int}/{omonth:int}/{oday:int}",
                    defaults: new { controller = "Order", action = "GetOrdersByDate" },
                   constraints: new { oyear = @"\d{4}", omonth = @"\d{2}", oday = @"\d{2}" });

                //// TODO L11 Task 2-1: Custom Routing Exercise 2
                //// Routing example: http://localhost/Shoppers/Product will display all products
                ////  and at the same times allow http://localhost/Shoppers/Product/{action}/{id} to access other 
                ////  actions in ProductController
                routes.MapControllerRoute(
                    name: "ShoppersProduct",
                    pattern: "Shoppers/Product/{action}/{id?}",
                    defaults: new { controller = "Product", action = "Index" });

                //// TODO L11 Task 2-1: Custom Routing Exercise 1
                //// Routing example: http://localhost/Shoppers will display all orders
                ////  and at the same times allow http://localhost/Shoppers/{action}/{id} to access other 
                ////  actions in OrderController
                routes.MapControllerRoute(
                    name: "ShoppersDefault",
                    pattern: "Shoppers/{action}/{id?}",
                    defaults: new { controller = "Order", action = "Index" });

                //// TODO L11 Solution Task: Add a custom routing to display notes for a specific lesson or a range lessons
                //// Sample web request http://localhost:62617/B215/1
                //// Sample Web request http://localhost:62617/B215/1/7
                //// The module must start with a case-insensitive alphabet followed by 3 digits
                //// The lesson ids must be integer
                routes.MapControllerRoute(
                    name: "ViewByModuleIdLessonId",
                    pattern: "{moduleId}/{frLessonId:int}/{toLessonId:int?}",
                    defaults: new { controller = "RPNotes", action = "ListByModuleLesson" },
                    constraints: new { moduleId = @"[A-Za-z]\d{3}" });

                //// TODO L11 Solution Task: Add a custom routing to display notes for a specific module
                //// Sample web request http://localhost:62617/B215
                //// The module must start with a case-insensitive alphabet followed by 3 digits
                routes.MapControllerRoute(
                    name: "ViewByModuleId",
                    pattern: "{moduleId}",
                    defaults: new { controller = "RPNotes", action = "ListByModule" },
                    constraints: new { moduleId = @"[A-Za-z]\d{3}" });

                //// TODO L11 Solution Task: Add a custom routing to allow viewing by module/topic/lesson hierarchy
                //// Sample web request http://localhost:62617/topic
                routes.MapControllerRoute(
                    name: "ViewByTopics",
                    pattern: "topic",
                    defaults: new { controller = "RPNotes", action = "TopicalIndex" });

                //// TODO L11 Solution Task: Add a custom routing to do searching by starting with a colon(:)
                //// Sample web request http://localhost:62617/:rules
                routes.MapControllerRoute(
                    name: "Search",
                    pattern: ":{keyPhrase}",
                    defaults: new { controller = "RPNotes", action = "Search" });

                //// TODO L11 Solution Task: Add a custom routing to display the main page at the same allow access to other actions within the RPNotesController
                //// Sample web request http://localhost:62617/ 
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
//19047572 Konada Obadiah Nahshon