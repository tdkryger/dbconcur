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
        /**
        Method uses planeNo to find a seat and ID to reserve a seat to this user.
        */
        public string reserve(string planeNo, long ID)
        {
            string seatNo = null;
            Utility.HandleConnection(delegate (MySqlConnection conn)
            {
                var selectCommand = new MySqlCommand("SELECT * FROM seat WHERE Reserved EQUALS NULL;", conn);
                var reader = selectCommand.ExecuteReader();

                reader.Read();

                seatNo = reader.GetString("seat_no");
                var updateCommand = new MySqlCommand("UPDATE seat set reserved = " + ID + " Where seat_no = " + seatNo, conn);
                int succes = updateCommand.ExecuteNonQuery();
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
