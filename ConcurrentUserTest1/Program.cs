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
        //private static int minThreads = 10;
        //private static int maxSleep = 1000;
        private static Random rnd = new Random();
        private static List<OurBackgroundWorker> workers = new List<OurBackgroundWorker>();
        private static DataObject data = new DataObject()
        {
            CurrentRunCount = 0,
            CurrentlyHighestId = -1,
            StartedThreads = 0,
            Run = true
        };
        //static int Reservations = 200;
        List<ReturnCode> bookresult = new List<ReturnCode>();

        public void ThreadPoolCallBack(Object ThreadContext)
        {
            int result = -5;
            Reservation res = new Reservation();
            long ID = (long)Thread.CurrentThread.ManagedThreadId;
            
            string seatNo = res.reserve("CR9", ID);
            Random r = new Random();
            int sleepTime = r.Next(0, 10000);
            Thread.Sleep(sleepTime);
            if (r.Next(0, 100) >= 75)
            {
                Console.Out.WriteLine("Thread {0} gave up on booking seat {1} and slept for {2} ms", ID, seatNo, sleepTime);
            }
            else
            {
                result = res.book("CR9", seatNo, ID);
                Console.Out.WriteLine("Thread {0} attempted to book seat {1}, after sleeping for {2} ms. Result: {3}.", ID, seatNo, sleepTime, result);
            }
                bookresult.Add((ReturnCode)result);
                res.CloseConnection();
        }

        static void Main(string[] args)
        {
            int i = 0;
            new Reservation().clearAllBookings(PLANE_NO);
            Program p = new Program();
            int minWorker, minIOS;

            ThreadPool.GetMinThreads(out minWorker, out minIOS);
            ThreadPool.SetMinThreads(10, minIOS);
            Reservation res = new Reservation();
            while (!res.isAllBooked(PLANE_NO))
            {
                i++;
                ThreadPool.QueueUserWorkItem(p.ThreadPoolCallBack, i);
                Thread.Sleep(100);
            }

            
             
            Thread.Sleep(11000);
            Console.WriteLine("Result =");
            p.bookresult.ForEach(x => Utility.HandleOutput(x.ToString()));
            Console.ReadLine();


            //    for (int i = 0; i < minThreads; i++)
            //    {
            //        initializeNewWorker();
            //        Thread.Sleep(500);
            //    }

            //    while (data.CurrentRunCount != 0)
            //    {
            //        Thread.Sleep(1000);
            //        Utility.HandleOutput(string.Format("Current threads: {0}", data.CurrentRunCount));
            //        Utility.HandleOutput(string.Format("Threads started: {0}", data.StartedThreads));
            //    }

            //    foreach (var worker in workers)
            //        Utility.HandleOutput(string.Format("The user {0}, reserved the seat {1}, and got this return code: {2}", worker.Id, worker.seatNo, worker.ReturnCode));

            //    Utility.HandleOutput("Done!");
            //    Console.ReadLine();
            //}

            //private static void initializeNewWorker()
            //{
            //    lock (data)
            //        data.CurrentlyHighestId++;

            //    var worker = new OurBackgroundWorker(data.CurrentlyHighestId);
            //    worker.DoWork += Program_DoWork;
            //    worker.RunWorkerCompleted += Program_RunWorkerCompleted;

            //    worker.RunWorkerAsync();
            //    workers.Add(worker);
            //}

            //private static void Program_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
            //{
            //    lock (data)
            //        data.CurrentRunCount--;
            //    if (data.Run)
            //        initializeNewWorker();
            //}

            //private static void Program_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
            //{
            //    Reservation res = new Reservation("", "");
            //    lock (data)
            //    {
            //        data.CurrentRunCount++;
            //        data.StartedThreads++;
            //        data.Run = !res.isAllBooked(PLANE_NO);
            //    }

            //    OurBackgroundWorker obw = sender as OurBackgroundWorker;
            //    obw.seatNo = res.reserve(PLANE_NO, obw.Id);
            //    if (string.IsNullOrEmpty(obw.seatNo))
            //    {
            //        obw.seatNo = "No reservation";
            //        obw.ReturnCode = 0;
            //        obw.Id = 123456789;
            //    }

            //    Thread.Sleep(rnd.Next(0, maxSleep));

            //    // 75%
            //    if (rnd.Next(4) != 0)
            //    {
            //        obw.ReturnCode = (ReturnCode)res.book(PLANE_NO, obw.seatNo, obw.Id);
            //    }

            //    // This is because of some restrictions on the db (max open connections...)
            //    res.CloseConnection();

        }
    }
}