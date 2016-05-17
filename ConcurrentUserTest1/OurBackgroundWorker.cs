using System.ComponentModel;

namespace ConcurrentUserTest1
{
    public class OurBackgroundWorker : BackgroundWorker
    {
        public long Id { get; set; }
        public string SeatNo { get; set; }
        public ReturnCode ReturnCode { get; set; }

        public OurBackgroundWorker(long id)
        {
            Id = id;
        }
    }
}