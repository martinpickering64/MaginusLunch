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
    public class MongoOrderRepositoryTests
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
        public void TestRepositoryConstruction()
        {

            var repo = new MongoOrderRepository(settings, AsyncRetryPolicy, RetryPolicy);

            Assert.IsNotNull(repo);

        }

        [TestMethod]
        public void TestCreateOrder()
        {
            var baseRepo = new MongoOrderRepository(settings, AsyncRetryPolicy, RetryPolicy);
            var repo = baseRepo as IOrderRepository;

            var order = new Order(Guid.NewGuid());

            repo.SaveOrder(order);

            var expected = baseRepo.FindAll(0, 1).FirstOrDefault();

            Assert.IsNotNull(expected, "Failed to retrieve any Order from Repository!");
            Assert.AreEqual(order.Id, expected.Id);
        }

        [TestMethod]
        public void TestCreateOrderAsync()
        {
            var baseRepo = new MongoOrderRepository(settings, AsyncRetryPolicy, RetryPolicy);
            var repo = baseRepo as IOrderRepository;

            var order = new Order(Guid.NewGuid());

            repo.SaveOrderAsync(order).ConfigureAwait(false).GetAwaiter().GetResult();

            var expected = baseRepo.FindAll(0, 1).FirstOrDefault();

            Assert.IsNotNull(expected, "Failed to retrieve any Order from Repository!");
            Assert.AreEqual(order.Id, expected.Id);
        }

        [TestMethod]
        public void TestGetOrderById()
        {
            var baseRepo = new MongoOrderRepository(settings, AsyncRetryPolicy, RetryPolicy);
            var repo = baseRepo as IOrderRepository;

            var order = new Order(Guid.NewGuid());
            repo.SaveOrder(order);

            var expected = repo.GetOrder(order.Id);

            Assert.AreEqual(order.Id, expected.Id);
        }

        [TestMethod]
        public void TestGetOrderByIdAsync()
        {
            var baseRepo = new MongoOrderRepository(settings, AsyncRetryPolicy, RetryPolicy);
            var repo = baseRepo as IOrderRepository;

            var order = new Order(Guid.NewGuid());
            repo.SaveOrder(order);

            var expected = repo.GetOrderAsync(order.Id).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.AreEqual(order.Id, expected.Id);
        }

        [TestMethod]
        public void When_no_orders_in_repo_GetOrder_returns_null()
        {
            var baseRepo = new MongoOrderRepository(settings, AsyncRetryPolicy, RetryPolicy);
            var repo = baseRepo as IOrderRepository;

            var testId = Guid.NewGuid();
            var expected = repo.GetOrder(testId);

            Assert.IsNull(expected);
        }

        [TestMethod]
        public void When_order_isnt_in_repo_GetOrder_returns_null()
        {
            var baseRepo = new MongoOrderRepository(settings, AsyncRetryPolicy, RetryPolicy);
            var repo = baseRepo as IOrderRepository;
            var order = new Order(Guid.NewGuid());
            repo.SaveOrder(order);

            var testId = Guid.NewGuid();
            var expected = repo.GetOrder(testId);

            Assert.IsNull(expected);
        }

        [TestMethod]
        public void When_no_orders_in_repo_GetOrderAsync_returns_null()
        {
            var baseRepo = new MongoOrderRepository(settings, AsyncRetryPolicy, RetryPolicy);
            var repo = baseRepo as IOrderRepository;

            var testId = Guid.NewGuid();
            var expected = repo.GetOrderAsync(testId).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsNull(expected);
        }

        [TestMethod]
        public void When_order_isnt_in_repo_GetOrderAsync_returns_null()
        {
            var baseRepo = new MongoOrderRepository(settings, AsyncRetryPolicy, RetryPolicy);
            var repo = baseRepo as IOrderRepository;
            var order = new Order(Guid.NewGuid());
            repo.SaveOrder(order);

            var testId = Guid.NewGuid();
            var expected = repo.GetOrderAsync(testId).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsNull(expected);
        }

        [TestMethod]
        public void When_repo_empty_GetOrders_returns_empty()
        {
            var baseRepo = new MongoOrderRepository(settings, AsyncRetryPolicy, RetryPolicy);
            var repo = baseRepo as IOrderRepository;

            var expected = repo.GetOrders(DateTime.UtcNow.Date);

            Assert.IsNotNull(expected);
            Assert.IsTrue(!expected.Any());
        }

        [TestMethod]
        public void When_repo_has_no_orders_for_date_GetOrders_returns_empty()
        {
            var baseRepo = new MongoOrderRepository(settings, AsyncRetryPolicy, RetryPolicy);
            var repo = baseRepo as IOrderRepository;
            var order = new Order(Guid.NewGuid())
            {
                DeliveryDate = new DateTime(2016,8,20)
            };
            repo.SaveOrder(order);

            var expected = repo.GetOrders(DateTime.UtcNow.Date);

            Assert.IsNotNull(expected);
            Assert.IsTrue(!expected.Any());
        }

        [TestMethod]
        public void When_repo_has_orders_for_date_GetOrders_returns_them()
        {
            var baseRepo = new MongoOrderRepository(settings, AsyncRetryPolicy, RetryPolicy);
            var repo = baseRepo as IOrderRepository;
            var testDate = new DateTime(year:2016, month:8, day:20,
                                        hour:0, minute:0, second:0, millisecond:0,
                                        kind: DateTimeKind.Utc);
            var order1 = new Order(Guid.NewGuid())
            {
                DeliveryDate = testDate
            };
            var order2 = new Order(Guid.NewGuid())
            {
                DeliveryDate = testDate
            };
            var order3 = new Order(Guid.NewGuid())
            {
                DeliveryDate = DateTime.UtcNow
            };
            repo.SaveOrder(order1);
            repo.SaveOrder(order2);
            repo.SaveOrder(order3);

            var expected = repo.GetOrders(testDate);

            Assert.IsNotNull(expected, "GetOrders should never return NULL");
            Assert.AreEqual(2, expected.Count());
        }
    }
}
