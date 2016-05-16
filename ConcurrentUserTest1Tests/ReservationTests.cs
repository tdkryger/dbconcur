using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConcurrentUserTest1.Tests
{
    [TestClass()]
    public class ReservationTests
    {
        private Reservation reservation;
        private string PLANE_NO = "CR9";
        private string USER_NAME = "test";
        private string PASSWORD = "testpass";
        private long TEST_ID = 123456789;
        
        [TestInitialize()]
        public void BeforeAll()
        {
            reservation = new Reservation(USER_NAME, PASSWORD);
            reservation.clearAllBookings(PLANE_NO);
        }
        
        [TestCleanup()]
        public void AfterAll()
        {
            reservation.clearAllBookings(PLANE_NO);
            reservation = null;
        }

        [TestMethod()]
        public void reserveTest()
        {
            var result = reservation.reserve(PLANE_NO, TEST_ID);
            Assert.IsFalse(string.IsNullOrWhiteSpace(result));
        }

        [TestMethod()]
        public void bookTest()
        {
            var seat = reservation.reserve(PLANE_NO, TEST_ID);
            var result = reservation.book(PLANE_NO, seat, TEST_ID);

            Assert.IsTrue(result == 0);
        }

        [TestMethod()]
        public void bookAllTest()
        {
            reservation.bookAll(PLANE_NO);

            Assert.IsTrue(reservation.isAllBooked(PLANE_NO));
        }

        [TestMethod()]
        public void isAllBookedTest()
        {
            Assert.IsFalse(reservation.isAllBooked(PLANE_NO));
            reservation.bookAll(PLANE_NO);
            Assert.IsTrue(reservation.isAllBooked(PLANE_NO));
        }

        [TestMethod()]
        public void isAllReservedTest()
        {
            Assert.IsFalse(reservation.isAllReserved(PLANE_NO));

            for (int i = 0; i < 96; i++)
            {
                reservation.reserve(PLANE_NO, TEST_ID);
            }

            Assert.IsTrue(reservation.isAllReserved(PLANE_NO));
        }
    }
}