using System;

namespace ReceiptStash.Models
{
    public class OrderModel
    {
        public uint orderID { get; set; }
        public uint storeID { get; set; }
        public string itemName { get; set; }
        public decimal itemPrice { get; set; }
        public uint quantity { get; set; }
        public string cardType { get; set; }
        public int cardNumber { get; set; }
        public DateTime date { get; set; }
    }
}
