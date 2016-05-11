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
            var command = new MySqlCommand("UPDATE seat reserved = NULL, booked = NULL, booking_time = NULL", conn);
            command.ExecuteNonQuery();
        }

        public string reserve(string planeNo, long id)
        {
            string seatNo = null;
            Utility.HandleConnection(delegate (MySqlConnection conn)
            {
                var selectCommand = new MySqlCommand("SELECT * FROM seat WHERE Reserved EQUALS NULL;", conn);
                var reader = selectCommand.ExecuteReader();

                reader.Read();

                seatNo = reader.GetString("seat_no");
                var updateCommand = new MySqlCommand("UPDATE seat set reserved = " + id + " Where seat_no = " + seatNo, conn);
                int succes = updateCommand.ExecuteNonQuery();
                //Console.Out.WriteLine(seatNo);
            });

            return seatNo;
        }

        public int book(string plane_no, string seat_no, long id)
        {
            var selectCommand = new MySqlCommand("SELECT * FROM seat WHERE plane_no = @plane_no AND seat_no = @seat_no", conn);
            selectCommand.Parameters.AddWithValue("plane_no", plane_no);
            selectCommand.Parameters.AddWithValue("seat_no", seat_no);
            var reader = selectCommand.ExecuteReader();

            // The specified seat is found!
            if (reader.HasRows)
            {
                reader.Read();
                int? reserved = (int?)reader.GetValue(2);
                int? booked = (int?)reader.GetValue(3);
                int? bookingTime = (int?)reader.GetValue(4);

                if (reserved == null)
                {
                    return -1;
                }
                else if (reserved != id)
                {
                    return -2;
                }

                if (bookingTime == 1)
                {
                    return -3;
                }

                // Fucked up time representation, this is not over!
                if (booked != null)
                {
                    return -4;
                }

                var updateCommand = new MySqlCommand("UPDATE seat SET booked = @id WHERE plane_no = @plane_no AND seat_no = @seat_no", conn);
                updateCommand.Parameters.AddWithValue("id", id);
                updateCommand.Parameters.AddWithValue("plane_no", plane_no);
                updateCommand.Parameters.AddWithValue("seat_no", seat_no);
                
                var result = updateCommand.ExecuteNonQuery();

                if (result == 1)// Rows affected
                {
                    return 0;
                }
                else
                {
                    return -5;
                }
            }
            else
            {
                return -5;
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
