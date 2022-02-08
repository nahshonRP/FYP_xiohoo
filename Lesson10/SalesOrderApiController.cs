using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lesson10.Models;

namespace Lesson10.Controllers
{

    [Route("api/Sales")]
    public class SalesOrderApiController : Controller
    {
        private AppDbContext _dbContext;

        public SalesOrderApiController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // TODO P10 Task 1-1 Implement GetAll action
        // This action returns all orders with Ok Http status code
        // Sample web request: http://localhost:60601/api/Sales
        // Sample web response:
        // [{"id":1,"productId":"ST0001","productName":"Silver  bracelet","price":12.5,"qty":4,"customerId":"C001","customer":null},
        //  {"id":2,"productId":"ST0032","productName":"Silver watch","price":15.0,"qty":5,"customerId":"P102","customer":null},
        //  {"id":3,"productId":"CU686","productName":"Copper earrings","price":27.0,"qty":3,"customerId":"C001","customer":null}]
        [HttpGet]
        public IActionResult GetAll()
        {
            DbSet<Order> dbs = _dbContext.Order;
            var orders = dbs.ToList();
            return Ok(orders);
        }

        // TODO P10 Task 1-3 Implement GetAllAmounts action
        // This action returns amounts sold for each order where amount = price x qty with Ok Http status code
        // Sample web request: http://localhost:60601/api/Sales/Amount
        // Sample web response: [50.0,75.0,81.0]
        [HttpGet("Amount")]
        public IActionResult GetAllAmounts()
        {
            DbSet<Order> dbs = _dbContext.Order;
            var amounts = dbs.Select(o => o.Price * o.Qty)
                .ToList();
            return Ok(amounts);

        }

        // TODO P10 Task 1-5 Implement GetAllCustomers action
        // This action returns all customers with Ok Http status code
        // Sample web request: http://localhost:60601/api/Sales/Customer
        // Sample web response:
        // [{"id":"C001","name":"Chai Hui Pte Lte","isCorporate":true,"order":[]},
        //  {"id":"P102","name":"Flores Dimangi","isCorporate":false,"order":[]}]
        [HttpGet("Customer")]
        public IActionResult GetAllCustomers()
        {
            DbSet<Customer> dbs = _dbContext.Customer;
            var customers = dbs.ToList();
            return Ok(customers);

        }

        // TODO P10 Task 1-7 Implement GetOrdersByCustomer action
        // This action takes in custId as string, return orders object of this customer with Http Ok status
        // Sample web request: http://localhost:60601/api/Sales/Customer/C001/Order
        // Sample web response:
        // [{"id":1,"productId":"ST0001","productName":"Silver bracelet","price":12.5,"qty":4,"customerId":"C001","customer":null},
        // {"id":3,"productId":"CU686","productName":"Copper earrings","price":27.0,"qty":3,"customerId":"C001","customer":null}]
        [HttpGet("Customer/{custId}/Order")]
        public IActionResult GetOrdersByCustomer(string custId)
        {
            DbSet<Order> dbs = _dbContext.Order;
            var orders = dbs.Where(o => o.CustomerId == custId).ToList();

            return Ok(orders);

        }

        // TODO P10 Task 2-1 Implement AddNewCustomer action
        // This action takes in receive a Customer object and update database
        // Sample web request: http://localhost:60601/api/Sales/Customer/Add
        // If there is already a customer with same id, return -1
        // If update to database is successful, return 1
        // If update to database failed, return 0
        [HttpPost("Customer/Add")]
        public IActionResult AddNewCustomer(Customer newCustomer)
        {
            DbSet<Customer> dbs = _dbContext.Customer;
            Customer customer = dbs.Where(c => c.Id == newCustomer.Id).FirstOrDefault();

            if (customer == null)
            {
                dbs.Add(newCustomer);
                if (_dbContext.SaveChanges() == 1)
                    return Ok(1);
                else
                    return Ok(0);
            }
            else
                return Ok(-1);

        }
    }
}
//19047572 Konada Obadiah Nahshon