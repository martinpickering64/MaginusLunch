using MaginusLunch.Orders.Domain;
using MaginusLunch.Orders.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;

namespace MaginusLunch.Orders.Service.UnitTests
{
    [TestCategory("Unit")]
    [TestClass]
    public class CalendarServiceTests
    {
        [TestMethod]
        public void TestDateInPastIsNotAvailable()
        {
            var repo = new Mock<ICalendarRepository>();

            var sus = new CalendarService(repo.Object);

            var expected = sus.IsAvailable(new DateTime(1996, 12, 25));

            Assert.IsFalse(expected);
        }

        [TestMethod]
        public void TestDateInPastIsWithdrawn()
        {
            var repo = new Mock<ICalendarRepository>();

            var sus = new CalendarService(repo.Object);

            var expected = sus.IsWithdrawn(new DateTime(1996, 12, 25));

            Assert.IsTrue(expected);
        }

        [TestMethod]
        public void TestTodayIsAvailable()
        {
            var mockRepo = new Mock<ICalendarRepository>();
            var repo = mockRepo.Object;
            var calendar = new Calendar(Guid.NewGuid());
            mockRepo.Setup(x => x.GetCalendar()).Returns(calendar);

            var sus = new CalendarService(repo);

            var expected = sus.IsAvailable(DateTime.Today);

            Assert.IsTrue(expected);
        }

        [TestMethod]
        public void TestTodayIsNotWithdrawn()
        {
            var mockRepo = new Mock<ICalendarRepository>();
            var repo = mockRepo.Object;
            var calendar = new Calendar(Guid.NewGuid());
            mockRepo.Setup(x => x.GetCalendar()).Returns(calendar);

            var sus = new CalendarService(repo);

            var expected = sus.IsWithdrawn(DateTime.Today);

            Assert.IsFalse(expected);
        }

        [TestMethod]
        public void TestSaturdayIsNotAvailable()
        {
            var mockRepo = new Mock<ICalendarRepository>();
            var repo = mockRepo.Object;
            var calendar = new Calendar(Guid.NewGuid());
            mockRepo.Setup(x => x.GetCalendar()).Returns(calendar);

            var saturday = DateTime.Today.AddDays(DayOfWeek.Saturday - DateTime.Today.DayOfWeek);

            var sus = new CalendarService(repo);

            var expected = sus.IsAvailable(saturday);

            Assert.IsFalse(expected);
        }

        [DataTestMethod]
        [DataRow(DayOfWeek.Saturday)]
        [DataRow(DayOfWeek.Sunday)]
        public void TestWeekendNotAvailable(DayOfWeek dayOfWeek)
        {
            var mockRepo = new Mock<ICalendarRepository>();
            var repo = mockRepo.Object;
            var calendar = new Calendar(Guid.NewGuid());
            mockRepo.Setup(x => x.GetCalendar()).Returns(calendar);

            var desiredDay = DateTime.Today.AddDays(7 + dayOfWeek - DateTime.Today.DayOfWeek);

            var sus = new CalendarService(repo);

            var expected = sus.IsAvailable(desiredDay);

            Assert.IsFalse(expected);
        }

        [DataTestMethod]
        [DataRow(DayOfWeek.Monday)]
        [DataRow(DayOfWeek.Tuesday)]
        [DataRow(DayOfWeek.Wednesday)]
        [DataRow(DayOfWeek.Thursday)]
        [DataRow(DayOfWeek.Friday)]
        public void TestWeekDayIsAvailable(DayOfWeek dayOfWeek)
        {
            var mockRepo = new Mock<ICalendarRepository>();
            var repo = mockRepo.Object;
            var calendar = new Calendar(Guid.NewGuid());
            mockRepo.Setup(x => x.GetCalendar()).Returns(calendar);

            var desiredDay = DateTime.Today.AddDays(7 + dayOfWeek - DateTime.Today.DayOfWeek);

            var sus = new CalendarService(repo);

            var expected = sus.IsAvailable(desiredDay);

            Assert.IsTrue(expected);
        }

        [TestMethod]
        public void TestBeforeUnavailableDay()
        {
            var mockRepo = new Mock<ICalendarRepository>();
            var repo = mockRepo.Object;
            var calendar = new Calendar(Guid.NewGuid());
            var withdrawnDate = DateTime.Today.AddDays(7 + DayOfWeek.Wednesday - DateTime.Today.DayOfWeek);
            calendar.WithdrawnDates.Add(withdrawnDate.Date);
            mockRepo.Setup(x => x.GetCalendar()).Returns(calendar);

            var sus = new CalendarService(repo);

            var expected = sus.IsAvailable(withdrawnDate.AddDays(-1));

            Assert.IsTrue(expected);
        }

        [TestMethod]
        public void TestAfterUnavailableDay()
        {
            var mockRepo = new Mock<ICalendarRepository>();
            var repo = mockRepo.Object;
            var calendar = new Calendar(Guid.NewGuid());
            var withdrawnDate = DateTime.Today.AddDays(7 + DayOfWeek.Wednesday - DateTime.Today.DayOfWeek);
            calendar.WithdrawnDates.Add(withdrawnDate.Date);
            mockRepo.Setup(x => x.GetCalendar()).Returns(calendar);

            var sus = new CalendarService(repo);

            var expected = sus.IsAvailable(withdrawnDate.AddDays(1));

            Assert.IsTrue(expected);
        }

        [TestMethod]
        public void TestAnUnavailableDay()
        {
            var mockRepo = new Mock<ICalendarRepository>();
            var repo = mockRepo.Object;
            var calendar = new Calendar(Guid.NewGuid());
            var withdrawnDate = DateTime.Today.AddDays(7 + DayOfWeek.Wednesday - DateTime.Today.DayOfWeek);
            calendar.WithdrawnDates.Add(withdrawnDate.Date.ToUniversalTime());
            mockRepo.Setup(x => x.GetCalendar()).Returns(calendar);

            var sus = new CalendarService(repo);

            var expected = sus.IsAvailable(withdrawnDate);

            Assert.IsFalse(expected);
        }
    }
}
