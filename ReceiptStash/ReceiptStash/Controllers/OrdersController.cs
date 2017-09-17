using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ReceiptStash.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

namespace ReceiptStash.Controllers
{
    [Route("[controller]")]
    [RestAuthorize]
    public class OrdersController : Controller
    {
        private IDatabaseModel _database;

        public OrdersController (IDatabaseModel database)
        {
            _database = database;
        }

        // GET orders/1/1
        // Returns the order identified by orderID if the account identified by userID is authenticated
        // Returns a failure if userID or orderID does not exist, or if userID does not own orderID
        [HttpGet("{userID}/{orderID}")]
        public string GetOrders(int userID, int orderID)
        {
            // do token authentication stuff with userID

            IEnumerable<OrderModel> records = _database.GetRecords(userID, orderID);

            if (records != null)
                return Success(records);
            else
                return Failure();
        }

        // GET orders/1
        // Returns all orders made by the account identified by userID
        // Returns a failure if userID does not exist
        [HttpGet("{userID}")]
        public string GetAllOrders(int userID)
        {
            // authentication

            IEnumerable<OrderModel> records = _database.GetAllRecords(userID);

            if (records != null)
                return Success(records);
            else
                return Failure();
        }

        // GET orders/1/recent/3
        // Returns all orders made after the orderID given
        // Returns a failure if userID does not exist
        [HttpGet("{userID}/recent/{lastOrderID}")]
        public string GetRecentOrders(int userID, int lastOrderID)
        {
            // authentication

            IEnumerable<OrderModel> records = _database.GetRecentRecords(userID, lastOrderID);

            if (records != null)
                return Success(records);
            else
                return Failure();
        }

        // POST orders
        // Adds the order identified by orderID to the account identified by userID if the account is authenticated
        // Returns a failure if either the userID or orderID does not exist
        [HttpPost("{orderID}")]
        public string LinkTransaction(int orderID, [FromBody]PostDataModel data)
        {
            if (_database.TransferRecords(data.userID, orderID))
                return Success(null);
            else
                return Failure();
        }

        // POST orders
        // Adds a new transaction to the orders table
        [HttpPost]
        public string AddOrder([FromBody]OrderModel order)
        {
            if (_database.AddRecords(order))
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
