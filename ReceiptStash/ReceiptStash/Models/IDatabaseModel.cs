using System.Collections.Generic;

namespace ReceiptStash.Models
{
    public interface IDatabaseModel
    {
        bool AddRecords(OrderModel order);
        bool TransferRecords(int userID, int orderID);
        IEnumerable<OrderModel> GetRecords(int userID, int orderID);
        IEnumerable<OrderModel> GetAllRecords(int userID);
        IEnumerable<OrderModel> GetRecentRecords(int userID, int orderID);
    }
}
