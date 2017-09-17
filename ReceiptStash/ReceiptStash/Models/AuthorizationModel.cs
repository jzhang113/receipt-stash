using System;
using System.Security.Cryptography;
using System.Text;
using System.Net.Http;
using MySql.Data.MySqlClient;

namespace ReceiptStash.Models
{
    public class AuthorizationModel
    {
        private const string _connectionString = "server=localhost;user=root;database=receipt_stash;password=aype9akp3;SslMode=none";
        private const string _alg = "HmacSHA256";
        private const string _salt = "jgONPHkiTFnjCamNtgor";
        private const int _expirationMinutes = 30;

        public static string GenerateToken(string username, string password, string userAgent, long timestamp)
        {
            string hash = string.Join(":", new string[] { username, userAgent, timestamp.ToString() });
            string hashLeft = "";
            string hashRight = "";

            using (HMAC hmac = HMACSHA256.Create(_alg))
            {
                hmac.Key = Encoding.UTF8.GetBytes(GetHashedPassword(password));
                hmac.ComputeHash(Encoding.UTF8.GetBytes(hash));
                hashLeft = Convert.ToBase64String(hmac.Hash);
                hashRight = string.Join(":", new string[] { username, timestamp.ToString() });
            }

            return Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Join(":", hashLeft, hashRight)));
        }

        public static string GetHashedPassword(string password)
        {
            string key = string.Join(":", new string[] { password, _salt });

            using (HMAC hmac = HMACSHA256.Create(_alg))
            {
                // Hash the key.
                hmac.Key = Encoding.UTF8.GetBytes(_salt);
                hmac.ComputeHash(Encoding.UTF8.GetBytes(key));
                return Convert.ToBase64String(hmac.Hash);
            }
        }

        public static bool IsTokenValid(string token, string userAgent)
        {
            bool result = false;

            try
            {
                // Base64 decode the string, obtaining the token:username:timeStamp.
                string key = Encoding.UTF8.GetString(Convert.FromBase64String(token));
                // Split the parts.
                string[] parts = key.Split(new char[] { ':' });

                if (parts.Length == 3)
                {
                    // Get the hash message, username, and timestamp.
                    string hash = parts[0];
                    string username = parts[1];
                    long ticks = long.Parse(parts[2]);
                    DateTime timeStamp = new DateTime(ticks);
                    
                    // Ensure the timestamp is valid.
                    bool expired = Math.Abs((DateTime.UtcNow - timeStamp).TotalMinutes) > _expirationMinutes;

                    if (!expired)
                    {
                        // Lookup the user's account from the db
                        
                        if (DatabaseModel.QueryExists("SELECT EXISTS(SELECT 1 FROM users WHERE username=@0);", username))
                        {
                            string cmd = "SELECT password FROM users WHERE username=@username";
                            string password = null;

                            try
                            {
                                using (MySqlConnection conn = new MySqlConnection(_connectionString))
                                using (MySqlCommand sqlCmd = new MySqlCommand(cmd, conn))
                                {
                                    conn.Open();
                                    sqlCmd.Parameters.AddWithValue("@username", username);

                                    using (MySqlDataReader dr = sqlCmd.ExecuteReader())
                                    {
                                        dr.Read();

                                        password = dr.GetString(0);
                                    }
                                }

                                // Hash the message with the key to generate a token.
                                string computedToken = GenerateToken(username, password, userAgent, ticks);
                                // Compare the computed token with the one supplied and ensure they match.
                                result = (token == computedToken);
                            }
                            catch (Exception)
                            {
                                result = false;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                // error handling

                Console.WriteLine(e.ToString());
            }

            return result;
        }
    }
}
