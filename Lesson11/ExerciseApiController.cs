using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Lesson11.Controllers
{
    [Route("api/Ex")]
    public class ExerciseApiController : Controller
    {
        // TODO Lesson11 Task 1-1 Paste your apiKey here
        private string _apiKey = "Ncni5PdXwuYTJkPgvhfQ2XjPLdi6mw";

        // TODO Lesson11 Task 1-2 Implement ConvertCurrency action
        // This action returns the raw result of the external web api with Ok Http status code
        // Sample web request: http://localhost:62617/api/Ex/Currency/SGD/USD
        // Sample web response:
        // {"error":0,"error_message":"-","amount":0.73497}
        [HttpGet("Currency/{frCur}/{toCur}")]
        public IActionResult ConvertCurrency(string frCur, string toCur)
        {
            string url = $"https://www.amdoren.com/api/currency.php?api_key=AefB5uWVaRCL6eY7XwyRabVktzcqH3&from=SGD&to=USD";

            dynamic data = WebUtl.CallWebApi(url);

            if (data != null)
            {
                var result = new
                {
                    from = frCur,
                    to = toCur,
                    rate = data.amount
                };
                return Ok(result);
            }
            else
                return BadRequest();
            // TODO Lesson11 Task 1-3 Modify ConvertCurrency action
            // Modify your code in task 1-2 to the raw result of the external web api in a different format with Ok Http status code
            // Sample web request: http://localhost:62617/api/Ex/Currency/SGD/USD
            // Sample web response:
            // {"from":"SGD","to":"USD","rate":0.73497}
        }

        // TODO Lesson11 Task 1-4 Implement ConvertCurrency with amount action
        // This action calls the currency web api to get the latest rate and calculate and return the converted amount
        // Sample web request: http://localhost:62617/api/Ex/Currency/SGD/USD/20
        // Sample web response:
        // 14.6994
        [HttpGet("Currency/{frCur}/{toCur}/{amount}")]
        public IActionResult ConvertCurrency(string frCur, string toCur, double amount)
        {
            string url = $"https://www.amdoren.com/api/currency.php?api_key=AefB5uWVaRCL6eY7XwyRabVktzcqH3&from=SGD&to=USD";

            dynamic data = WebUtl.CallWebApi(url);

            if (data != null)
            {
                var result = data.amount * amount;
                return Ok();
            }
            else
                return BadRequest();
        }

        // TODO Lesson11 Task 1-5 Implement Get5DaysWeather action
        // This action calls the weather web api and return the 5 days forecast in an arry
        // Sample web request: http://localhost:62617/api/Ex/weather/1.4433892/103.7854804
        // Sample web response:
        // [{"date":"2018-07-28","avg_c":30,"min_c":26,"max_c":33,
        //   "avg_f":86,"min_f":79,"max_f":91,"summary":"Partly cloudy",
        //   "icon":"wi_color_partly_cloudy_day.png"},
        //  {"date":"2018-07-29","avg_c":28,"min_c":24,"max_c":32,
        //   "avg_f":82,"min_f":75,"max_f":90,"summary":"Light rain",
        //   "icon":"wi_color_drizzle.png"},
        //  {"date":"2018-07-30","avg_c":28,"min_c":24,"max_c":30,
        //   "avg_f":82,"min_f":75,"max_f":86,"summary":"Partly cloudy",
        //   "icon":"wi_color_partly_cloudy_day.png"},
        //  {"date":"2018-07-31","avg_c":27,"min_c":24,"max_c":30,
        //   "avg_f":81,"min_f":75,"max_f":86,"summary":"Rain",
        //   "icon":"wi_color_drizzle.png"},
        //  {"date":"2018-08-01","avg_c":25,"min_c":23,"max_c":28,
        //   "avg_f":77,"min_f":73,"max_f":82,"summary":"Rain",
        //   "icon":"wi_color_drizzle.png"}]
        [HttpGet("Weather/{latitude}/{longitude}")]
        public IActionResult Get5DaysWeather(double latitude, double longitude)
        {
            string url = $"https://www.amdoren.com/api/weather.php?api_key=AefB5uWVaRCL6eY7XwyRabVktzcqH3&lat=30.592849&lon=114.305539";

            dynamic data = WebUtl.CallWebApi(url);

            if (data != null)
            {
                return Ok(data.forecast);
            }
            else
                return BadRequest();
        }
    }
}
//19047572 Konada Obadiah Nahshon