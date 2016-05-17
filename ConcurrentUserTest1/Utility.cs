using MySql.Data.MySqlClient;
using System;

namespace ConcurrentUserTest1
{
    public class Utility
    {
        private static string CONNECTION_STRING = string.Format("Server = {0}; Database = {1}; Uid = {2}; Pwd = {3};",
                                                                "77.66.48.115",
                                                                "cluster_b",
                                                                "cluster_b",
                                                                "password");
        public static void HandleConnection(Action<MySqlConnection> method)
        {
            using (MySqlConnection conn = new MySqlConnection())
            {
                conn.ConnectionString = CONNECTION_STRING;
                conn.Open();
                method(conn);
            }
        }

        public static MySqlConnection GetConnection()
        {
            MySqlConnection connection = new MySqlConnection();

            connection.ConnectionString = CONNECTION_STRING;
            connection.Open();

            return connection;
        }

        public static void HandleOutput(string output)
        {
            Console.WriteLine(string.Format(output));
        }
    }
}