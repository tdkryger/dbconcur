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

            var selectCommand = new MySqlCommand("SELECT * FROM seat WHERE reserved = NULL AND booked = null AND booking_time = null", conn);
            var reader = selectCommand.ExecuteReader();

            reader.Read();

            if (reader.HasRows)
            {
                var seat_no = reader.GetString("seat_no");
                var updateCommand = new MySqlCommand("UPDATE seat SET reserved = @id, booking_time = @booking_time WHERE plane_no = @plane_no AND seat_no = @seat_no", conn);
                updateCommand.Parameters.AddWithValue("id", id);
                updateCommand.Parameters.AddWithValue("booking_time", 1223233); // ?????
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
                int? reserved = (int?)reader.GetValue(2);
                int? booked = (int?)reader.GetValue(3);
                int? bookingTime = (int?)reader.GetValue(4);

                if (reserved == null)
                {
                    return (int)ReturnCode.SeatNotReserved;
                }
                else if (reserved != id)
                {
                    return (int)ReturnCode.SeatNotReservedForUser;
                }

                // Fucked up time representation, this is not over!
                if (bookingTime == 1)
                {
                    return (int)ReturnCode.ReservationTimeout;
                }

                if (booked != null)
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
            var command = new MySqlCommand("UPDATE seat SET reserved = 12345, booked = 12345, booking_time = 12345 WHERE plane_no = @plane_no", conn);
            command.Parameters.AddWithValue("plane_no", plane_no);
            command.ExecuteNonQuery();
        }

        public bool isAllBooked(string plane_no)
        {
            var command = new MySqlCommand("SELECT booked FROM seat WHERE plane_no = @plane_no", conn);
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                if (reader.GetValue(3) == null)
                    return false;
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
                    return false;
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