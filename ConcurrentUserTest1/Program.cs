using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConcurrentUserTest1
{
    class Program
    {
        static int currentRunCount = 0;
        static int RunCount = 10;
        static int counter = 0;
        static object lockObject = new object();
        static Random rnd = new Random();
        static int Reservations = 200;
        List<int> bookresult = new List<int>();

        static System.ComponentModel.BackgroundWorker[] workers = new System.ComponentModel.BackgroundWorker[RunCount];

        public void ThreadPoolCallBack(Object ThreadContext)
        {

            Reservation res = new Reservation("User", "Password");
            long ID = (long)Thread.GetDomainID();
            Console.Out.WriteLine("Reserving");
            string seatNo = res.reserve("CR9", ID);
            Random r = new Random();
            Thread.Sleep(r.Next(0,10000));
            if (r.Next(0, 100) >= 75)
            {
                
            }
            else
            {
                int result = res.book("CR9", seatNo, ID);
                bookresult.Add(result);


            }
            
          
        }

        static void Main(string[] args)
        {
            Program p = new Program();
            int minWorker, minIOS;

            ThreadPool.GetMinThreads(out minWorker, out minIOS);



            ThreadPool.SetMinThreads(4, minIOS);

            for (int i = 0; i < Reservations; i++)
            {
                ThreadPool.QueueUserWorkItem(p.ThreadPoolCallBack, i);
            }
            Thread.Sleep(100000);

            Console.Out.WriteLine(p.bookresult);



            //        initializeWorkers();
            //        for (int i = 0; i < workers.Length; i++)
            //        {
            //            if (!workers[i].IsBusy)
            //                workers[i].RunWorkerAsync(i);
            //        }
            //        while (counter < 50)
            //        {
            //            Thread.Sleep(2);
            //            Console.WriteLine("Running workers: " + currentRunCount);
            //        }

            //        Console.WriteLine("Done");
            //        Console.ReadLine();
            //    }

            //    static void initializeWorkers()
            //    {


            //        for (int i = 0; i < workers.Length; i++)
            //        {
            //            workers[i] = new System.ComponentModel.BackgroundWorker();
            //            workers[i].DoWork += Program_DoWork;
            //            workers[i].RunWorkerCompleted += Program_RunWorkerCompleted;
            //        }
            //    }

            //    private static void Program_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
            //    {
            //        int idx = (int)e.Result;
            //        lock (lockObject)
            //        {
            //            //Console.WriteLine(e.Result + ": " + counter);
            //            currentRunCount--;
            //        }
            //        if (!workers[idx].IsBusy)
            //            workers[idx].RunWorkerAsync(idx);
            //    }

            //    private static void Program_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
            //    {
            //        lock (lockObject)
            //        {
            //            currentRunCount++;
            //            Thread.Sleep(rnd.Next(20, 400));
            //            e.Result = (int)e.Argument;
            //            counter++;
            //        }
            //    }

            //}
        }
    }
    }
