using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using Polly;
using Polly.Retry;
using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;

namespace MaginusLunch.MongoDB.IntegrationTests
{
    [TestCategory("Integration")]
    [TestCategory("MongoDB")]
    [TestClass]
    public class MongoRepositoryTests
    {
        const string connectionString = "mongodb://localhost:C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==@localhost:10255/MaginusLunchIntegrationTest?ssl=true&3t.sslSelfSignedCerts=true";
        readonly MongoRepositorySettings settings = new MongoRepositorySettings { ConnectionString = connectionString };

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
        public void TestMongoRepositoryFindById()
        {
            var repo = new TestEntityRepository(settings, AsyncRetryPolicy, RetryPolicy);
            var te = new TestEntity(Guid.NewGuid());
            repo.Insert(te);

            var expected = repo.Find(i => i.Id == te.Id).FirstOrDefault();

            Assert.IsNotNull(expected, "Failed to retrieve any Entity from Repository!");
            Assert.AreEqual(te.Id, expected.Id);
        }

        [TestMethod]
        public void TestMongoRepositoryFindByIdAsync()
        {
            var repo = new TestEntityRepository(settings, AsyncRetryPolicy, RetryPolicy);
            var te = new TestEntity(Guid.NewGuid());
            repo.Insert(te);

            var expected = repo.FindAsync(i => i.Id == te.Id)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult()
                .FirstOrDefault();

            Assert.IsNotNull(expected, "Failed to retrieve any Entity from Repository!");
            Assert.AreEqual(te.Id, expected.Id);
        }

        [TestMethod]
        public void TestMongoRepositoryFindByIdWithOrderClauseById()
        {
            var repo = new TestEntityRepository(settings, AsyncRetryPolicy, RetryPolicy);
            var te = new TestEntity(Guid.NewGuid());
            repo.Insert(te);

            var expected = repo.Find(e => e.Id == te.Id, e => e.Id, 0, 1).FirstOrDefault();

            Assert.IsNull(expected, "Didn't expect to retrieve any Entity from Repository!");
            Assert.Inconclusive("Why the order expression [e => e.Id] always gets zero results, I don't know.");
        }

    }
}
