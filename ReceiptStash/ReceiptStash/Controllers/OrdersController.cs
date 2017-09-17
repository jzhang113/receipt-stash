using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ReceiptStash.Models;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ReceiptStash.Controllers
{
    [Route("[controller]")]
    public class OrdersController : Controller
    {
        /**
        Implement dependency injection first

        private IDatabaseModel database;

        public OrdersController (DatabaseModel database)
        {
            this.database = database;
        }
    */

        // GET orders/1/1
        // Returns the order identified by orderID if the account identified by userID is authenticated
        // Returns a failure if userID or orderID does not exist, or if userID does not own orderID
        [HttpGet("{userID}/{orderID}")]
        public string GetOrders(int userID, int orderID)
        {
            // do token authentication stuff with userID

            IDatabaseModel database = new DatabaseModel();
            return Success(database.GetRecords(orderID));
        }

        // GET orders/1
        // Returns all orders made by the account identified by userID
        // Returns a failure if userID does not exist
        [HttpGet("{userID}")]
        public string GetAllOrders(int userID)
        {
            IDatabaseModel database = new DatabaseModel();
            return Success(database.GetAllRecords(userID));
        }

        // GET orders/1/recent/3
        // Returns all orders made after the orderID given
        // Returns a failure if userID does not exist
        [HttpGet("{userID}/recent/{lastOrderID}")]
        public string GetRecentOrders(int userID, int lastOrderID)
        {
            IDatabaseModel database = new DatabaseModel();
            return Success(database.GetRecentRecords(userID, lastOrderID));
        }

        // POST orders
        // Adds the order identified by orderID to the account identified by userID if the account is authenticated
        // Returns a failure if either the userID or orderID does not exist
        [HttpPost("{orderID}")]
        public string LinkTransaction(int orderID, [FromBody]PostDataModel data)
        {
            IDatabaseModel database = new DatabaseModel();
            if (database.TransferRecords(data.userID, orderID))
                return Success(null);
            else
                return Failure();
        }

        // POST orders
        // Adds a new order to the orders table
        [HttpPost]
        public string AddOrder([FromBody]OrderModel order)
        {
            IDatabaseModel database = new DatabaseModel();
            if (database.AddRecords(order))
                return Success(null);
            else
                return Failure();
        }

        private static string Success(dynamic body)
        {
            return "{" +
                "\"status\":\"success\"," +
                "\"data\":" + JsonConvert.SerializeObject(body) +
                "}";
        }

        private static string Failure()
        {
            return "{" +
                "\"status\":\"failure\"," +
                "\"data\":null" +
                "}";
        }
    }
}
