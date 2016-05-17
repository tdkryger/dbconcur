using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ConcurrentUserTest1
{
    public class DataObject
    {
        public int CurrentRunCount { get; set; }
        public int StartedThreads { get; set; }
        public int FinishedThreads { get; set; }
        public long CurrentlyHighestId { get; set; }
        public bool Run { get; set; }
    }

    public class Program
    {
        private static string PLANE_NO = "CR9";
        private static int minThreads = 10;
        private static int maxSleep = 10000;
        private static Random rnd = new Random();
        private static List<OurBackgroundWorker> workers = new List<OurBackgroundWorker>();
        private static DataObject data = new DataObject()
        {
            CurrentRunCount = 0,
            CurrentlyHighestId = -1,
            StartedThreads = 0,
            FinishedThreads = 0,
            Run = true
        };

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.Unicode;
            Utility.HandleOutput("Starting...");

            new Reservation("", "").clearAllBookings(PLANE_NO);

            for (int i = 0; i <= minThreads; i++)
            {
                initializeNewWorker();
                Thread.Sleep(100);
            }

            while (data.CurrentRunCount != 0)
            {
                Thread.Sleep(250);
                Console.Clear();
                Utility.HandleOutput(string.Format("Current threads: {0}", data.CurrentRunCount));
                Utility.HandleOutput(string.Format("Started threads: {0}", data.StartedThreads));
                Utility.HandleOutput(string.Format("Finished threads: {0}", data.FinishedThreads));
            }

            var queryable = workers.AsQueryable();
            var succesfullBookings = queryable.Count(x => x.ReturnCode == ReturnCode.SuccessfulBooking);
            var bookingsWithoutReservations = queryable.Count(x => x.ReturnCode == ReturnCode.SeatNotReserved);
            var notReservedForUser = queryable.Count(x => x.ReturnCode == ReturnCode.SeatNotReservedForUser);
            var seatAlreadyOccupied = queryable.Count(x => x.ReturnCode == ReturnCode.SeatAlreadyOccupied);

            var error = queryable.Count(x => x.ReturnCode == ReturnCode.Error);
            var timeout = queryable.Count(x => x.ReturnCode == ReturnCode.ReservationTimeout);
            var defaultReturn = queryable.Count(x => x.ReturnCode == ReturnCode.Default);


            Utility.HandleOutput("-----------------------------------------------------------------------------------------------");
            Utility.HandleOutput(string.Format("Number of UserThreads started: {0}", data.StartedThreads));
            Utility.HandleOutput(string.Format("Number of successful bookings: {0} ({1}%)", succesfullBookings, CalcPer(succesfullBookings)));
            Utility.HandleOutput(string.Format("Number of bookings without a reservation: {0} ({1}%)", bookingsWithoutReservations, CalcPer(bookingsWithoutReservations)));
            Utility.HandleOutput(string.Format("Number of bookings where the customer was not the one that held the reservation: {0} ({1}%)", notReservedForUser, CalcPer(notReservedForUser)));
            Utility.HandleOutput(string.Format("Number of bookings of occupied seats: {0} ({1}%)", seatAlreadyOccupied, CalcPer(seatAlreadyOccupied)));
            Utility.HandleOutput(string.Format("Number of bookings with unspecified errors: {0} ({1}%)", error, CalcPer(error)));
            Utility.HandleOutput(string.Format("Number of bookings which encountered a timeout: {0} ({1}%)", timeout, CalcPer(timeout)));
            Utility.HandleOutput(string.Format("Number of reservations, with no following booking: {0} ({1}%)", defaultReturn, CalcPer(defaultReturn)));
            Utility.HandleOutput("-----------------------------------------------------------------------------------------------");
            Utility.HandleOutput("Done!");
            Console.ReadLine();
        }

        private static double CalcPer(int part)
        {
            return Math.Round(((double)part / data.FinishedThreads) * 100.0, 2, MidpointRounding.AwayFromZero);
        }

        private static void initializeNewWorker()
        {
            lock (data)
                data.CurrentlyHighestId++;

            var worker = new OurBackgroundWorker(data.CurrentlyHighestId);
            worker.DoWork += Program_DoWork;
            worker.RunWorkerCompleted += Program_RunWorkerCompleted;

            worker.RunWorkerAsync();
            workers.Add(worker);
        }

        private static void Program_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            lock (data)
            {
                data.CurrentRunCount--;
                data.FinishedThreads++;
            }
            if (data.Run)
                initializeNewWorker();
        }

        private static void Program_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            Reservation res = new Reservation("", "");
            lock (data)
            {
                data.CurrentRunCount++;
                data.StartedThreads++;
                data.Run = !res.isAllBooked(PLANE_NO);
            }

            OurBackgroundWorker obw = sender as OurBackgroundWorker;
            obw.SeatNo = res.reserve(PLANE_NO, obw.Id);

            Thread.Sleep(rnd.Next(0, maxSleep));

            // 75% chooses to book their reservation
            if (rnd.Next(4) != 0)
                obw.ReturnCode = (ReturnCode)res.book(PLANE_NO, obw.SeatNo, obw.Id);
            else
                obw.ReturnCode = ReturnCode.Default;

            res.CloseConnection();
        }
    }
}