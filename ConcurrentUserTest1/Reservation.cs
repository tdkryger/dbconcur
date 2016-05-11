using MySql.Data.MySqlClient;
using System;

namespace ConcurrentUserTest1
{
    public class Reservation
    {
        private MySqlConnection conn;
        private string user;
        private string pw;
        private int timeout;

        public Reservation(string user, string pw)
        {
            conn = Utility.GetConnection();
            this.user = user;
            this.pw = pw;
            timeout = -5;
        }

        public void clearAllBookings(string plane_no)
        {
            var command = new MySqlCommand("UPDATE seat SET reserved = NULL, booked = NULL, booking_time = NULL WHERE plane_no = @plane_no", conn);
            command.Parameters.AddWithValue("plane_no", plane_no);
            command.ExecuteNonQuery();
        }

        public string reserve(string planeNo, long id)
        {
            //string seatNo = null;
            //Utility.HandleConnection(delegate (MySqlConnection conn)
            //{
            //    var selectCommand = new MySqlCommand("SELECT * FROM seat WHERE Reserved EQUALS NULL;", conn);
            //    var reader = selectCommand.ExecuteReader();

            //    reader.Read();

            //    seatNo = reader.GetString("seat_no");
            //    var updateCommand = new MySqlCommand("UPDATE seat set reserved = " + id + " Where seat_no = " + seatNo, conn);
            //    int succes = updateCommand.ExecuteNonQuery();
            //    //Console.Out.WriteLine(seatNo);
            //});

            //return seatNo;

            var selectCommand = new MySqlCommand("SELECT * FROM seat WHERE reserved IS NULL AND booked IS NULL AND booking_time IS NULL", conn);

            string seat_no;
            using (MySqlDataReader reader = selectCommand.ExecuteReader())
            {
                reader.Read();
                seat_no = reader.HasRows ? reader.GetString("seat_no") : string.Empty;
            }

            if (!string.IsNullOrEmpty(seat_no))
            {
                var updateCommand = new MySqlCommand("UPDATE seat SET reserved = @id, booking_time = @booking_time WHERE plane_no = @plane_no AND seat_no = @seat_no", conn);
                updateCommand.Parameters.AddWithValue("id", id);
                updateCommand.Parameters.AddWithValue("booking_time", DateTime.Now);
                updateCommand.Parameters.AddWithValue("plane_no", planeNo);
                updateCommand.Parameters.AddWithValue("seat_no", seat_no);
                updateCommand.ExecuteNonQuery();

                return seat_no;
            }
            else
            {
                return null;
            }
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
                int reserved = reader.IsDBNull(2) ? -1337 : reader.GetInt32("reserved");
                int booked = reader.IsDBNull(3) ? -1337 : reader.GetInt32("booked");
                DateTime bookingTime = reader.GetDateTime("booking_time");
                reader.Close();
                if (reserved == -1337)
                {
                    return (int)ReturnCode.SeatNotReserved;
                }
                else if (reserved != id)
                {
                    return (int)ReturnCode.SeatNotReservedForUser;
                }

                if (DateTime.Compare(bookingTime, DateTime.Now.AddMinutes(timeout)) >= 0)
                {
                    return (int)ReturnCode.ReservationTimeout;
                }

                if (booked != -1337)
                {
                    return (int)ReturnCode.SeatAlreadyOccupied;
                }

                var updateCommand = new MySqlCommand("UPDATE seat SET booked = @id, booking_time = @booking_time WHERE plane_no = @plane_no AND seat_no = @seat_no", conn);
                updateCommand.Parameters.AddWithValue("id", id);
                updateCommand.Parameters.AddWithValue("booking_time", bookingTime);
                updateCommand.Parameters.AddWithValue("plane_no", plane_no);
                updateCommand.Parameters.AddWithValue("seat_no", seat_no);

                var result = updateCommand.ExecuteNonQuery();

                if (result == 1)// Rows affected
                {
                    return (int)ReturnCode.SuccefulBooking;
                }
                else
                {
                    return (int)ReturnCode.Error;
                }
            }
            else
            {
                return (int)ReturnCode.Error;
            }
        }

        public void bookAll(string plane_no)
        {
            var command = new MySqlCommand("UPDATE seat SET reserved = 12345, booked = 12345, booking_time = @booking_time WHERE plane_no = @plane_no", conn);
            command.Parameters.AddWithValue("plane_no", plane_no);
            command.Parameters.AddWithValue("booking_time", DateTime.Now);
            command.ExecuteNonQuery();
        }

        public bool isAllBooked(string plane_no)
        {
            var command = new MySqlCommand("SELECT booked FROM seat WHERE plane_no = @plane_no", conn);
            command.Parameters.AddWithValue("plane_no", plane_no);
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                if (reader.IsDBNull(0))
                {
                    reader.Close();
                    return false;
                }
            }

            return true;
        }

        public bool isAllReserved(string plane_no)
        {
            var command = new MySqlCommand("SELECT * FROM seat WHERE plane_no = @plane_no", conn);
            command.Parameters.AddWithValue("plane_no", plane_no);
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                if (reader.GetValue(2) == null)
                {
                    reader.Close();
                    return false;
                }
            }

            return true;
        }
    }

    public enum ReturnCode
    {
        SuccefulBooking = 0,
        SeatNotReserved = -1,
        SeatNotReservedForUser = -2,
        ReservationTimeout = -3,
        SeatAlreadyOccupied = -4,
        Error = -5
    }
}