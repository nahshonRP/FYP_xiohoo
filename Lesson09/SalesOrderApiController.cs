using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Lesson09.Models;


namespace Lesson09.Controllers
{
    [Route("api/Sales")]
    public class SalesOrderApiController : Controller
    {
        private List<Order> orders = new List<Order>()
        {
            new Order { ProductId="ST0001",
                        ProductName="Silver bracelet",
                        Price =12.50,
                        Qty =4 },
            new Order { ProductId="ST0032",
                        ProductName="Silver watch",
                        Price =15,
                        Qty =5 },
            new Order { ProductId="CU686",
                        ProductName="Copper earrings",
                        Price =27,
                        Qty =3 }
        };

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(orders);
        }

        [HttpGet("Price/{id}")]
        public IActionResult GetPrice(string id)
        {
            Order order = orders
                           .Where(o => o.ProductId == id)
                           .FirstOrDefault();
            if (order != null)
                return Ok(order.Price);
            else
                return BadRequest();
        }

        // TODO Lesson09 Task 2-1 Implement Get action
        // This action takes in id as string, which represents ProductId, and return the corresponding order object
        // If the order found, return the order with Ok Http status code
        // If the order not found, return Bad Request Http status code
        // Sample web request: http://localhost:60601/api/Sales/ST0001
        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            Order order = orders.Where(o => o.ProductId == id).FirstOrDefault();
            if (order != null)
                return Ok(order);
            else
                return BadRequest();
        }

        // TODO Lesson09 Task 2-2 Implement GetName action
        // This action takes in id as string, which represents ProductId, and return the corresponding order's ProductName property
        // If order is found, return the ProductName with Ok Http status code
        // If the order not found, return Bad Request Http status code
        // Sample web request: http://localhost:60601/api/Sales/Name/ST0032
        [HttpGet("Name/{id}")]
        public IActionResult GetName(string id)
        {
            Order order = orders.Where(o => o.ProductId == id).FirstOrDefault();
            if (order != null)
                return Ok($"\"{order.ProductName}\"");
            else
                return BadRequest();
        }

    }
}
//19047572 Konada Obadiah Nahshon