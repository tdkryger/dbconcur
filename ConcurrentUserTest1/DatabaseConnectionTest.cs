using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConcurrentUserTest1
{
    public class DatabaseConnectionTest
    {
        static string myConnectionString;

        public static void Connect(Action<MySqlConnection> method)
        {
            MySqlConnection conn = null;
            myConnectionString = string.Format("Server = {0}; Database = {1}; Uid = {2}; Pwd = {3};",
                                                        "77.66.48.115",
                                                        "cluster_b",
                                                        "cluster_b",
                                                        "password");

            try
            {
                conn = new MySqlConnection();
                conn.ConnectionString = myConnectionString;
                conn.Open();
                method(conn);
                conn.Close();
            }
            catch (MySqlException ex)
            {
                Console.Out.WriteLine(ex.Message);
            }
        }
    }
}