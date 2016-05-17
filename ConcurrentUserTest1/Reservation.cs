using MySql.Data.MySqlClient;
using System;

namespace ConcurrentUserTest1
{
    public class Reservation
    {
        private MySqlConnection conn;
        private MySqlTransaction dbTrans;
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

        ~Reservation()
        {
            CloseConnection();
        }

        public void CloseConnection()
        {
            conn.Close();
        }

        public void clearAllBookings(string plane_no)
        {
            var command = new MySqlCommand("UPDATE seat SET reserved = NULL, booked = NULL, booking_time = NULL WHERE plane_no = @plane_no", conn);
            command.Parameters.AddWithValue("plane_no", plane_no);
            command.ExecuteNonQuery();
        }

        public string reserve(string planeNo, long id)
        {
            dbTrans = conn.BeginTransaction();
            var selectCommand = new MySqlCommand("SELECT * FROM seat WHERE (reserved IS NULL AND booked IS NULL AND booking_time IS NULL) OR (booked IS NULL AND booking_time < @current_time) LOCK IN SHARE MODE", conn);
            selectCommand.Parameters.AddWithValue("current_time", DateTime.Now.AddSeconds(timeout));

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
                dbTrans.Commit();
                return seat_no;
            }
            else
            {
                dbTrans.Rollback();
                return null;
            }
        }

        public int book(string plane_no, string seat_no, long id)
        {
            dbTrans = conn.BeginTransaction();
            var selectCommand = new MySqlCommand("SELECT * FROM seat WHERE plane_no = @plane_no AND seat_no = @seat_no LOCK IN SHARE MODE", conn);
            selectCommand.Parameters.AddWithValue("plane_no", plane_no);
            selectCommand.Parameters.AddWithValue("seat_no", seat_no);
            var reader = selectCommand.ExecuteReader();

            // The specified seat is found!
            if (reader.HasRows)
            {
                reader.Read();
                int reserved = reader.IsDBNull(2) ? -1337 : reader.GetInt32("reserved");
                int booked = reader.IsDBNull(3) ? -1337 : reader.GetInt32("booked");
                DateTime bookingTime = reader.IsDBNull(4) ? DateTime.MinValue : reader.GetDateTime("booking_time");
                reader.Close();
                if (reserved == -1337)
                {
                    dbTrans.Rollback();
                    return (int)ReturnCode.SeatNotReserved;
                }
                else if (reserved != id)
                {
                    dbTrans.Rollback();
                    return (int)ReturnCode.SeatNotReservedForUser;
                }

                if (DateTime.Compare(bookingTime, DateTime.Now.AddMinutes(timeout)) <= 0)
                {
                    dbTrans.Rollback();
                    return (int)ReturnCode.ReservationTimeout;
                }

                if (booked != -1337)
                {
                    dbTrans.Rollback();
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
                    dbTrans.Commit();
                    return (int)ReturnCode.SuccefulBooking;
                }
                else
                {
                    dbTrans.Rollback();
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
            var command = new MySqlCommand("SELECT booked FROM seat WHERE plane_no = @plane_no AND booked IS NULL", conn);

            command.Parameters.AddWithValue("plane_no", plane_no);

            var reader = command.ExecuteReader();
            var result = reader.HasRows;

            reader.Close();

            return !result;
        }

        public bool isAllReserved(string plane_no)
        {
            var command = new MySqlCommand("SELECT reserved FROM seat WHERE plane_no = @plane_no AND reserved IS NULL", conn);

            command.Parameters.AddWithValue("plane_no", plane_no);

            var reader = command.ExecuteReader();
            var result = reader.HasRows;

            reader.Close();

            return !result;
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