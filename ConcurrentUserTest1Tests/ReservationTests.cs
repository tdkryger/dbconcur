using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConcurrentUserTest1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ConcurrentUserTest1.Tests
{
    [TestClass()]
    public class ReservationTests
    {
        [TestMethod()]
        public void ReservationTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void clearAllBookingsTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void reserveTest()
        {
            Reservation res = new Reservation("", "");

            var result = res.reserve("CR9", 123);

            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void bookTest()
        {
            Reservation res = new Reservation("", "");

            var result = res.book("CR9", "A10", 123);

            Assert.IsTrue(result == 0);
        }

        [TestMethod()]
        public void bookAllTest()
        {
            var res = new Reservation("", "");

            res.bookAll("CR9");

            Assert.IsTrue(res.isAllBooked("CR9"));
        }

        [TestMethod()]
        public void isAllBookedTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void isAllReservedTest()
        {
            Assert.Fail();
        }
    }
}