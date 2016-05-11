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

        public int book(string plane_no, string seat_no, long id)
        {
            //Make sure that the id has reserved the seat, and then attempt to book it
            switch (id)
            {
                case 0:
                    //Booked successfully
                    return 0;
                case -1:
                    //Not reserved at all
                    return -1;
                case -2:
                    //Not reserved by this customer id
                    return -2;
                case -3:
                    //Reservation timeout
                    return -3;
                case -4:
                    //Seat is already booked
                    return -4;
                case -5:
                    //All other errors
                    return -5;
                default:
                    return 1;
            }
        }

        public void bookAll(string plane_no)
        {

        }

        public bool isAllBooked(string plane_no)
        {
            var command = new MySqlCommand("SELECT plane_no FROM seat where plane_no = @plane_no and ;", conn);
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var seatNo = reader.GetString("seat_no");
                Console.Out.WriteLine(seatNo);
            }
            return true;
        }

        public bool isAllReserved(string plane_no)
        {
            var command = new MySqlCommand("SELECT * FROM seat;", conn);
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var seatNo = reader.GetString("seat_no");
                Console.Out.WriteLine(seatNo);
            }
            return true;
        }
    }
}
