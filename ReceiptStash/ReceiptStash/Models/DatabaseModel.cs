using System;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace ReceiptStash.Models
{
    public class DatabaseModel : IDatabaseModel
    {
        private const string _connectionString = "server=localhost;user=root;database=receipt_stash;password=aype9akp3;SslMode=none";

        public bool AddRecords(OrderModel order)
        {
            string cmd = "INSERT INTO orders " +
                "(id, store_id, item_name, item_price, quantity, card_type, card_number, date) " +
                "values (@orderID, @storeID, @itemName, @itemPrice, @quantity, @cardType, @cardNumber, @date);";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(_connectionString))
                using (MySqlCommand sqlCmd = new MySqlCommand(cmd, conn))
                {
                    conn.Open();
                    sqlCmd.Parameters.AddWithValue("@orderID", order.orderID);
                    sqlCmd.Parameters.AddWithValue("@storeID", order.storeID);
                    sqlCmd.Parameters.AddWithValue("@itemName", order.itemName);
                    sqlCmd.Parameters.AddWithValue("@itemPrice", order.itemPrice);
                    sqlCmd.Parameters.AddWithValue("@quantity", order.quantity);
                    sqlCmd.Parameters.AddWithValue("@cardType", order.cardType);
                    sqlCmd.Parameters.AddWithValue("@cardNumber", order.cardNumber);
                    sqlCmd.Parameters.AddWithValue("@date", order.date);
                    sqlCmd.ExecuteNonQueryAsync();
                }

                return true;
            }
            catch (Exception e)
            {
                // Error handling stuff 

                Console.Write(e.ToString());
                return false;
            }
        }

        public bool TransferRecords(int userID, int orderID)
        {
            if (!QueryExists("SELECT EXISTS(SELECT 1 FROM users WHERE id=@0);", userID) || !QueryExists("SELECT EXISTS(SELECT 1 FROM orders WHERE id=@0);", orderID))
                return false;            

            string cmd = "INSERT INTO userorders " +
                "(user_id, order_id) " +
                "values (@userID, @orderID);";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(_connectionString))
                using (MySqlCommand sqlCmd = new MySqlCommand(cmd, conn))
                {
                    conn.Open();
                    sqlCmd.Parameters.AddWithValue("@userID", userID);
                    sqlCmd.Parameters.AddWithValue("@orderID", orderID);
                    sqlCmd.ExecuteNonQueryAsync();
                }

                return true;
            }
            catch (Exception e)
            {
                // Error handling stuff

                Console.Write(e.ToString());
                return false;
            }
        }

        public IEnumerable<OrderModel> GetRecords(int userID, int orderID)
        {
            if (!QueryExists("SELECT EXISTS(SELECT 1 FROM users WHERE id=@0);", userID) || !QueryExists("SELECT EXISTS(SELECT 1 FROM orders WHERE id=@0);", orderID))
                return null;

            if (!QueryExists("SELECT EXISTS(SELECT 1 from userorders where user_id=@0 and order_id=@1);", userID, orderID))
                return null;

            return Query("SELECT * " +
                "FROM orders WHERE id=@0;", orderID);
        }

        public IEnumerable<OrderModel> GetAllRecords(int userID)
        {
            if (!QueryExists("SELECT EXISTS(SELECT 1 FROM users WHERE id=@0);", userID))
                return null;

            return Query("SELECT orders.* " +
                "FROM userorders " +
                "JOIN orders ON order_id = orders.id " +
                "WHERE user_id=@0;", userID);
        }

        public IEnumerable<OrderModel> GetRecentRecords(int userID, int orderID)
        {
            if (!QueryExists("SELECT EXISTS(SELECT 1 FROM users WHERE id=@0);", userID))
                return null;

            return Query("SELECT orders.* " +
                "FROM userorders " +
                "JOIN orders ON order_id = orders.id " +
                "WHERE user_id=@0 AND order_id>@1;", userID, orderID);
        }

        internal static IEnumerable<OrderModel> Query(string cmd, params dynamic[] args)
        {
            ICollection<OrderModel> orderList = new List<OrderModel>();

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            using (MySqlCommand sqlCmd = new MySqlCommand(cmd, conn))
            {
                conn.Open();
                
                for (int i = 0; i < args.Length; i++)
                {
                    sqlCmd.Parameters.AddWithValue("@" + i, args[i]);
                }

                using (MySqlDataReader dr = sqlCmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        orderList.Add(new OrderModel
                        {
                            orderID = dr.GetUInt32(0),
                            storeID = dr.GetUInt32(1),
                            itemName = dr.GetString(2),
                            itemPrice = dr.GetDecimal(3),
                            quantity = dr.GetUInt32(4),
                            cardType = dr.GetString(5),
                            cardNumber = dr.GetInt32(6),
                            date = dr.GetDateTime(7)
                        });
                    }
                }
            }

            return orderList;
        }

        internal static bool QueryExists(string cmd, params dynamic[] args)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            using (MySqlCommand sqlCmd = new MySqlCommand(cmd, conn))
            {
                conn.Open();

                for (int i = 0; i < args.Length; i++)
                {
                    sqlCmd.Parameters.AddWithValue("@" + i, args[i]);
                }

                using (MySqlDataReader dr = sqlCmd.ExecuteReader())
                { 
                    // we should only have 1 row
                    dr.Read();

                    if (dr.GetBoolean(0))
                       return true;
                    else
                       return false;
                }
            }
        }
    }
}
