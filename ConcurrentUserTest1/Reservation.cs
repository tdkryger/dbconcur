using MySql.Data.MySqlClient;
using System;

namespace ConcurrentUserTest1
{
    public class Reservation
    {
        private MySqlConnection conn;
        private string user;
        private string pw;

        public Reservation(string user, string pw)
        {
            this.user = user;
            this.pw = pw;
            conn = Utility.GetConnection();
        }

        public void clearAllBookings(string plane_no)
        {
        }

        public String reserve(String planeNo, long ID)
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

        public bool isAllReserved(string plane_no)
        {
            return true;
        }
    }
}
