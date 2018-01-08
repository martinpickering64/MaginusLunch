using MaginusLunch.MongoDB;
using MaginusLunch.Orders.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using Polly;
using Polly.Retry;
using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;

namespace MaginusLunch.Orders.Repository.IntegrationTests
{
    [TestCategory("Integration")]
    [TestCategory("MongoDB")]
    [TestClass]
    public class MongoCalendarRepositoryTests
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
        public void TestRepositoryConstruction()
        {

            var repo = new MongoCalendarRepository(repositorySettings, AsyncRetryPolicy, RetryPolicy);

            Assert.IsNotNull(repo);

        }

        [TestMethod]
        public void TestCreateCalendar()
        {
            var baseRepo = new MongoCalendarRepository(repositorySettings, AsyncRetryPolicy, RetryPolicy);
            var repo = baseRepo as ICalendarRepository;

            var cal = new Calendar(Guid.NewGuid());

            repo.SaveCalendar(cal);

            var expected = baseRepo.FindAll(0, 1).FirstOrDefault();

            Assert.IsNotNull(expected, "Failed to retrieve any Calendar from Repository!");
            Assert.AreEqual(cal.Id, expected.Id);
        }

        [TestMethod]
        public void TestCreateCalendarAsync()
        {
            var baseRepo = new MongoCalendarRepository(repositorySettings, AsyncRetryPolicy, RetryPolicy);
            var repo = baseRepo as ICalendarRepository;

            var cal = new Calendar(Guid.NewGuid());

            repo.SaveCalendarAsync(cal).ConfigureAwait(false).GetAwaiter().GetResult();

            var expected = baseRepo.FindAll(0, 1).FirstOrDefault();

            Assert.IsNotNull(expected, "Failed to retrieve any Calendar from Repository!");
            Assert.AreEqual(cal.Id, expected.Id);
        }

        [TestMethod]
        public void TestGetCalendar()
        {
            var baseRepo = new MongoCalendarRepository(repositorySettings, AsyncRetryPolicy, RetryPolicy);
            var repo = baseRepo as ICalendarRepository;

            var cal = new Calendar(Guid.NewGuid());
            repo.SaveCalendar(cal);

            var expected = repo.GetCalendar();

            Assert.AreEqual(cal.Id, expected.Id);
        }

        [TestMethod]
        public void TestGetCalendarAsync()
        {
            var baseRepo = new MongoCalendarRepository(repositorySettings, AsyncRetryPolicy, RetryPolicy);
            var repo = baseRepo as ICalendarRepository;

            var cal = new Calendar(Guid.NewGuid());
            repo.SaveCalendar(cal);

            var expected = repo.GetCalendarAsync().ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.AreEqual(cal.Id, expected.Id);
        }

        [TestMethod]
        public void TestGetCalendarById()
        {
            var baseRepo = new MongoCalendarRepository(repositorySettings, AsyncRetryPolicy, RetryPolicy);
            var repo = baseRepo as ICalendarRepository;

            var cal = new Calendar(Guid.NewGuid());
            repo.SaveCalendar(cal);

            var expected = repo.GetCalendar(cal.Id);

            Assert.AreEqual(cal.Id, expected.Id);
        }

        [TestMethod]
        public void TestGetCalendarByIdAsync()
        {
            var baseRepo = new MongoCalendarRepository(repositorySettings, AsyncRetryPolicy, RetryPolicy);
            var repo = baseRepo as ICalendarRepository;

            var cal = new Calendar(Guid.NewGuid());
            repo.SaveCalendar(cal);

            var expected = repo.GetCalendarAsync(cal.Id).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.AreEqual(cal.Id, expected.Id);
        }
    }
}
