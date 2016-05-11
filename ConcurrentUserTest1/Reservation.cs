using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConcurrentUserTest1
{
    class Reservation
    {


        public Reservation(String User, String PW)
        {

        }

        public string reserve(string planeNo, long ID)
        {
            string seatNo = null;
            DatabaseConnectionTest.Connect(delegate (MySqlConnection conn)
            {
                var selectCommand = new MySqlCommand("SELECT * FROM seat WHERE Reserved EQUALS NULL;", conn);
                var reader = selectCommand.ExecuteReader();

                reader.Read();

                seatNo = reader.GetString("seat_no");
                var updateCommand = new MySqlCommand("UPDATE seat set reserved = " + ID + " Where seat_no = " + seatNo, conn);
                int succes = updateCommand.Executed

                //Console.Out.WriteLine(seatNo);
            });

            return seatNo;
        }

        public int book(String planeNo, String seatNo, long ID)
        {
            return 0;
        }
    }
}
