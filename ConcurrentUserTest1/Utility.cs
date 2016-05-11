using MySql.Data.MySqlClient;
using System;

namespace ConcurrentUserTest1
{
    public class Utility
    {
        public static void HandleConnection(Action<MySqlConnection> method)
        {
            using (MySqlConnection conn = new MySqlConnection())
            {
                conn.ConnectionString = string.Format("Server = {0}; Database = {1}; Uid = {2}; Pwd = {3};",
                                                            "77.66.48.115",
                                                            "cluster_b",
                                                            "cluster_b",
                                                            "password");
                conn.Open();
                method(conn);
            }

        }
    }
}