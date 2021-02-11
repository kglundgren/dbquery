using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace dbquery
{
    public class DatabaseConnection
    {
        private readonly string connectionString;

        public DatabaseConnection()
        {
            connectionString = Environment.GetEnvironmentVariable("localdb");
        }

        public string GetConnectionString()
        {
            // Crashes without explanation! 
            //return ConfigurationManager.ConnectionStrings["localdb"].ConnectionString;
            return connectionString;
        }

        public async Task TestConnection()
        {
            await using (var connection = new SqlConnection(connectionString)) {
                connection.Open();
                Console.WriteLine("Connection to database successful.");
            }
        }

        public async Task Query(string queryString)
        {
            await using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(queryString, conn)) {
                conn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader()) {
                    while (dr.Read()) {
                        Console.WriteLine($"Id: {dr["Id"]}, Name: {dr["Name"]}");
                    }
                }
            }
        }
    }
}
