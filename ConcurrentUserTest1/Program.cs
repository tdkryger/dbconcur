using System;
using System.Collections.Generic;
using System.Threading;

namespace ConcurrentUserTest1
{
    public class DataObject
    {
        public int CurrentRunCount { get; set; }
        public int StartedThreads { get; set; }
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
            Run = true
        };

        static void Main(string[] args)
        {
            new Reservation("", "").clearAllBookings(PLANE_NO);
            for (int i = 0; i < minThreads; i++)
            {
                initializeNewWorker();
                Thread.Sleep(500);
            }

            while (data.CurrentRunCount != 0)
            {
                Thread.Sleep(1000);
                Utility.HandleOutput(string.Format("Current threads: {0}", data.CurrentRunCount));
                Utility.HandleOutput(string.Format("Threads started: {0}", data.StartedThreads));
            }

            workers.ForEach(worker => Utility.HandleOutput(string.Format("The user {0}, reserved the seat {1}, and got this return code: {2}", worker.Id, worker.seatNo, worker.ReturnCode)));

            Utility.HandleOutput("Done!");
            Console.ReadLine();
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
                data.CurrentRunCount--;
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
            obw.seatNo = res.reserve(PLANE_NO, obw.Id);
            if (string.IsNullOrEmpty(obw.seatNo))
            {
                Utility.HandleOutput("No seat reserved for user " + obw.Id);
            }

            Thread.Sleep(rnd.Next(0, maxSleep));

            // 75%
            if (rnd.Next(4) != 0)
            {
                obw.ReturnCode = (ReturnCode)res.book(PLANE_NO, obw.seatNo, obw.Id);
            }

            // This is because of some restrictions on the db (max open connections...)
            res.CloseConnection();
        }
    }
}