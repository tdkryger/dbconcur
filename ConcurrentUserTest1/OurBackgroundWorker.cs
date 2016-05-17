using System.ComponentModel;

namespace ConcurrentUserTest1
{
    public class OurBackgroundWorker : BackgroundWorker
    {
        public ReturnCode ReturnCode { get; set; }
        public long Id { get; set; }
        public string seatNo { get; set; }

        public OurBackgroundWorker(long id)
        {
            Id = id;
        }
    }
}