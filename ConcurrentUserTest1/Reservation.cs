using System;

namespace ConcurrentUserTest1
{
    public class Reservation
    {
        string user;
        string pw;

        public Reservation(string user, string pw)
        {
            this.user = user;
            this.pw = pw;
        }

        public string reserve(string plane_no, long id)
        {
            //Return free seat where plane_no = plane_no and reserved = null and booked = null

            return "";
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

        public void clearAllBookings(string plane_no)
        {
        }

        public bool isAllBooked(string plane_no)
        {
            return true;
        }

        public bool isAllReserved(string plane_no)
        {
            return true;
        }
    }
}
