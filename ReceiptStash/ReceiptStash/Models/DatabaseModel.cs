using System;
using System.Data;
using System.Data.SqlClient;

namespace ReceiptStash.Models
{
    public class DatabaseModel : IDatabaseModel
    {
        private static SqlConnection _conn;

        static DatabaseModel()
        {
            _conn = new SqlConnection("Data Source=(local);Integrated Security=SSPI");
        }
    }
}
