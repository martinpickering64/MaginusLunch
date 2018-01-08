using MaginusLunch.MongoDB;
using MaginusLunch.Orders.Domain;
using MaginusLunch.Orders.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using Polly;
using Polly.Retry;
using System;
using System.IO;
using System.Net.Sockets;

namespace MaginusLunch.Orders.Service.IntegrationTests
{
    [TestCategory("Integration")]
    [TestCategory("MongoDB")]
    [TestClass]
    public class CalendarServiceIntegrationTests
    {
        const string connectionString = "mongodb://localhost:C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==@localhost:10255/MaginusLunchIntegrationTest?ssl=true&3t.sslSelfSignedCerts=true";
        readonly MongoRepositorySettings repositorySettings = new MongoRepositorySettings { ConnectionString = connectionString };

        private RetryPolicy AsyncRetryPolicy = Policy.Handle<MongoConnectionException>(i =>
                        i.InnerException.GetType() == typeof(IOException)
                        || i.InnerException.GetType() == typeof(SocketException))
                    .RetryAsync(3);
        private RetryPolicy RetryPolicy = Policy.Handle<MongoConnectionException>(i =>
                                i.InnerException.GetType() == typeof(IOException)
                                || i.InnerException.GetType() == typeof(SocketException))
                            .Retry(3);

        [TestCleanup]
        public void TestCleanup()
        {
            var client = new MongoClient(connectionString);
            client.DropDatabase("MaginusLunchIntegrationTest");
        }

        [TestMethod]
        public void TestDateInPastIsNotAvailable()
        {
            var repo = new MongoCalendarRepository(repositorySettings, AsyncRetryPolicy, RetryPolicy);
            var sus = new CalendarService(repo);

            var actual = sus.IsAvailable(new DateTime(1996, 12, 25));

            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void TestDateInPastIsWithdrawn()
        {
            var repo = new MongoCalendarRepository(repositorySettings, AsyncRetryPolicy, RetryPolicy);
            var sus = new CalendarService(repo);

            var actual = sus.IsWithdrawn(new DateTime(1996, 12, 25));

            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TestTodayIsAvailable()
        {
            var repo = new MongoCalendarRepository(repositorySettings, AsyncRetryPolicy, RetryPolicy) as ICalendarRepository;
            var sus = new CalendarService(repo);
            var calendar = new Calendar(Guid.NewGuid());
            repo.SaveCalendar(calendar);

            var actual = sus.IsAvailable(DateTime.Today);

            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TestTodayIsNotWithdrawn()
        {
            var repo = new MongoCalendarRepository(repositorySettings, AsyncRetryPolicy, RetryPolicy) as ICalendarRepository;
            var sus = new CalendarService(repo);
            var calendar = new Calendar(Guid.NewGuid());
            repo.SaveCalendar(calendar);

            var actual = sus.IsWithdrawn(DateTime.Today);

            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void TestSaturdayIsNotAvailable()
        {
            var repo = new MongoCalendarRepository(repositorySettings, AsyncRetryPolicy, RetryPolicy) as ICalendarRepository;
            var sus = new CalendarService(repo);
            var calendar = new Calendar(Guid.NewGuid());
            repo.SaveCalendar(calendar);
            var saturday = DateTime.Today.AddDays(DayOfWeek.Saturday - DateTime.Today.DayOfWeek);

            var actual = sus.IsAvailable(saturday);

            Assert.IsFalse(actual);
        }

        [DataTestMethod]
        [DataRow(DayOfWeek.Saturday)]
        [DataRow(DayOfWeek.Sunday)]
        public void TestWeekendNotAvailable(DayOfWeek dayOfWeek)
        {
            var repo = new MongoCalendarRepository(repositorySettings, AsyncRetryPolicy, RetryPolicy) as ICalendarRepository;
            var sus = new CalendarService(repo);
            var calendar = new Calendar(Guid.NewGuid());
            repo.SaveCalendar(calendar);
            var desiredDay = DateTime.Today.AddDays(7 + dayOfWeek - DateTime.Today.DayOfWeek);

            var actual = sus.IsAvailable(desiredDay);

            Assert.IsFalse(actual);
        }

        [DataTestMethod]
        [DataRow(DayOfWeek.Monday)]
        [DataRow(DayOfWeek.Tuesday)]
        [DataRow(DayOfWeek.Wednesday)]
        [DataRow(DayOfWeek.Thursday)]
        [DataRow(DayOfWeek.Friday)]
        public void TestWeekDayIsAvailable(DayOfWeek dayOfWeek)
        {
            var repo = new MongoCalendarRepository(repositorySettings, AsyncRetryPolicy, RetryPolicy) as ICalendarRepository;
            var sus = new CalendarService(repo);
            var calendar = new Calendar(Guid.NewGuid());
            repo.SaveCalendar(calendar);
            var desiredDay = DateTime.Today.AddDays(7 + dayOfWeek - DateTime.Today.DayOfWeek);

            var actual = sus.IsAvailable(desiredDay);

            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TestBeforeUnavailableDay()
        {
            var repo = new MongoCalendarRepository(repositorySettings, AsyncRetryPolicy, RetryPolicy) as ICalendarRepository;
            var sus = new CalendarService(repo);
            var calendar = new Calendar(Guid.NewGuid());
            var withdrawnDate = DateTime.Today.AddDays(7 + DayOfWeek.Wednesday - DateTime.Today.DayOfWeek);
            calendar.WithdrawnDates.Add(withdrawnDate.Date);
            repo.SaveCalendar(calendar);

            var actual = sus.IsAvailable(withdrawnDate.AddDays(-1));

            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TestAfterUnavailableDay()
        {
            var repo = new MongoCalendarRepository(repositorySettings, AsyncRetryPolicy, RetryPolicy) as ICalendarRepository;
            var sus = new CalendarService(repo);
            var calendar = new Calendar(Guid.NewGuid());
            var withdrawnDate = DateTime.Today.AddDays(7 + DayOfWeek.Wednesday - DateTime.Today.DayOfWeek);
            calendar.WithdrawnDates.Add(withdrawnDate.Date);
            repo.SaveCalendar(calendar);

            var actual = sus.IsAvailable(withdrawnDate.AddDays(1));

            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TestAnUnavailableDay()
        {
            var repo = new MongoCalendarRepository(repositorySettings, AsyncRetryPolicy, RetryPolicy) as ICalendarRepository;
            var sus = new CalendarService(repo);
            var calendar = new Calendar(Guid.NewGuid());
            var withdrawnDate = DateTime.Today.AddDays(7 + DayOfWeek.Wednesday - DateTime.Today.DayOfWeek);
            calendar.WithdrawnDates.Add(withdrawnDate.Date);
            repo.SaveCalendar(calendar);

            var actual = sus.IsAvailable(withdrawnDate);

            Assert.IsFalse(actual);
        }
    }
}
