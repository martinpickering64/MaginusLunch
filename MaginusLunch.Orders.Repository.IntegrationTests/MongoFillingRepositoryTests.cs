using MaginusLunch.MongoDB;
using MaginusLunch.Orders.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;

namespace MaginusLunch.Orders.Repository.IntegrationTests
{
    [TestCategory("Integration")]
    [TestCategory("MongoDB")]
    [TestClass]
    public class MongoFillingRepositoryTests
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

            var repo = new MongoFillingRepository(settings, AsyncRetryPolicy, RetryPolicy);

            Assert.IsNotNull(repo);

        }

        [TestMethod]
        public void TestCreateFilling()
        {
            var baseRepo = new MongoFillingRepository(settings, AsyncRetryPolicy, RetryPolicy);
            var repo = baseRepo as IFillingRepository;

            var filling = new Filling(Guid.NewGuid());

            repo.SaveFilling(filling);

            var expected = baseRepo.FindAll(0, 1).FirstOrDefault();

            Assert.IsNotNull(expected, "Failed to retrieve any Filling from Repository!");
            Assert.AreEqual(filling.Id, expected.Id);
        }

        [TestMethod]
        public void TestCreateFillingAsync()
        {
            var baseRepo = new MongoFillingRepository(settings, AsyncRetryPolicy, RetryPolicy);
            var repo = baseRepo as IFillingRepository;

            var filling = new Filling(Guid.NewGuid());

            repo.SaveFillingAsync(filling).ConfigureAwait(false).GetAwaiter().GetResult();

            var expected = baseRepo.FindAll(0, 1).FirstOrDefault();

            Assert.IsNotNull(expected, "Failed to retrieve any Filling from Repository!");
            Assert.AreEqual(filling.Id, expected.Id);
        }

        [TestMethod]
        public void TestGetFillingById()
        {
            var baseRepo = new MongoFillingRepository(settings, AsyncRetryPolicy, RetryPolicy);
            var repo = baseRepo as IFillingRepository;

            var filling = new Filling(Guid.NewGuid());
            repo.SaveFilling(filling);

            var expected = repo.GetFilling(filling.Id);

            Assert.AreEqual(filling.Id, expected.Id);
        }

        [TestMethod]
        public void TestGetFillingByIdAsync()
        {
            var baseRepo = new MongoFillingRepository(settings, AsyncRetryPolicy, RetryPolicy);
            var repo = baseRepo as IFillingRepository;

            var filling = new Filling(Guid.NewGuid());
            repo.SaveFilling(filling);

            var expected = repo.GetFillingAsync(filling.Id).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.AreEqual(filling.Id, expected.Id);
        }

        [TestMethod]
        public void Only_available_fillings_should_be_returned()
        {
            var baseRepo = new MongoFillingRepository(settings, AsyncRetryPolicy, RetryPolicy);
            var repo = baseRepo as IFillingRepository;

            var withdrawnId = Guid.NewGuid();
            var filling = new Filling(withdrawnId) { Status = FillingStatus.Withdrawn };
            repo.SaveFilling(filling);
            filling = new Filling(Guid.NewGuid());
            repo.SaveFilling(filling);
            filling = new Filling(Guid.NewGuid());
            repo.SaveFilling(filling);
            filling = new Filling(Guid.NewGuid());
            repo.SaveFilling(filling);
            var testDate = DateTime.Today.AddDays(10).ToUniversalTime();
            var embargoedId = Guid.NewGuid();
            filling = new Filling(embargoedId);// { WithdrawnDates = new List<DateTime> { testDate } };
            filling.WithdrawnDates.Add(testDate);
            repo.SaveFilling(filling);

            var actual = repo.GetAllFillings(testDate);

            Assert.IsFalse(actual.Any(f => f.Id == withdrawnId), "mistake: a withdrawn item was returned!");
            Assert.IsFalse(actual.Any(f => f.Id == embargoedId), "mistake: an embargoed item was returned!");
            Assert.IsFalse(actual.Any(f => f.WithdrawnDates.Contains(testDate)), "mistake: an embargoed item (by date) was returned!");
            Assert.AreEqual(3, actual.Count());
        }

        [TestMethod]
        public void Only_available_fillings_should_be_returned_Async()
        {
            var baseRepo = new MongoFillingRepository(settings, AsyncRetryPolicy, RetryPolicy);
            var repo = baseRepo as IFillingRepository;

            var withdrawnId = Guid.NewGuid();
            var filling = new Filling(withdrawnId) { Status = FillingStatus.Withdrawn };
            repo.SaveFilling(filling);
            filling = new Filling(Guid.NewGuid());
            repo.SaveFilling(filling);
            filling = new Filling(Guid.NewGuid());
            repo.SaveFilling(filling);
            filling = new Filling(Guid.NewGuid());
            repo.SaveFilling(filling);
            var testDate = DateTime.Today.AddDays(10).ToUniversalTime();
            var embargoedId = Guid.NewGuid();
            filling = new Filling(embargoedId);// { WithdrawnDates = new List<DateTime> { testDate } };
            filling.WithdrawnDates.Add(testDate);
            repo.SaveFilling(filling);

            var actual = repo.GetAllFillingsAsync(testDate).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsFalse(actual.Any(f => f.Id == withdrawnId), "mistake: a withdrawn item was returned!");
            Assert.IsFalse(actual.Any(f => f.Id == embargoedId), "mistake: an embargoed item was returned!");
            Assert.IsFalse(actual.Any(f => f.WithdrawnDates.Contains(testDate)), "mistake: an embargoed item (by date) was returned!");
            Assert.AreEqual(3, actual.Count());
        }

        [TestMethod]
        public void Only_available_fillings_for_category_should_be_returned()
        {
            var baseRepo = new MongoFillingRepository(settings, AsyncRetryPolicy, RetryPolicy);
            var repo = baseRepo as IFillingRepository;

            var testCategory = Guid.NewGuid();
            var withdrawnId = Guid.NewGuid();
            var filling = new Filling(withdrawnId)
            {
                Status = FillingStatus.Withdrawn,
            };
            filling.MenuCategories.Add(testCategory);
            repo.SaveFilling(filling);
            filling = new Filling(Guid.NewGuid()); // { MenuCategories = new List<Guid> { testCategory } };
            filling.MenuCategories.Add(testCategory);
            repo.SaveFilling(filling);
            filling = new Filling(Guid.NewGuid());
            repo.SaveFilling(filling);
            filling = new Filling(Guid.NewGuid()); // { MenuCategories = new List<Guid> { testCategory } };
            filling.MenuCategories.Add(testCategory);
            repo.SaveFilling(filling);
            var testDate = DateTime.Today.AddDays(10).ToUniversalTime();
            var embargoedId = Guid.NewGuid();
            filling = new Filling(embargoedId);
            filling.WithdrawnDates.Add(testDate);
            filling.MenuCategories.Add(testCategory);
            repo.SaveFilling(filling);

            var actual = repo.GetAllFillings(activeOnThisDate: testDate, menuCategoryId: testCategory);

            Assert.IsFalse(actual.Any(f => f.Id == withdrawnId), "mistake: a withdrawn item was returned!");
            Assert.IsFalse(actual.Any(f => f.Id == embargoedId), "mistake: an embargoed item was returned!");
            Assert.IsFalse(actual.Any(f => f.WithdrawnDates.Contains(testDate)), "mistake: an embargoed item (by date) was returned!");
            Assert.AreEqual(2, actual.Count());
        }

        [TestMethod]
        public void Only_available_fillings_for_category_should_be_returnedAsync()
        {
            var baseRepo = new MongoFillingRepository(settings, AsyncRetryPolicy, RetryPolicy);
            var repo = baseRepo as IFillingRepository;

            var testCategory = Guid.NewGuid();
            var withdrawnId = Guid.NewGuid();
            var filling = new Filling(withdrawnId)
            {
                Status = FillingStatus.Withdrawn,
            };
            filling.MenuCategories.Add(testCategory);
            repo.SaveFilling(filling);
            filling = new Filling(Guid.NewGuid());
            filling.MenuCategories.Add(testCategory);
            repo.SaveFilling(filling);
            filling = new Filling(Guid.NewGuid());
            repo.SaveFilling(filling);
            filling = new Filling(Guid.NewGuid());
            filling.MenuCategories.Add(testCategory);
            repo.SaveFilling(filling);
            var testDate = DateTime.Today.AddDays(10).ToUniversalTime();
            var embargoedId = Guid.NewGuid();
            filling = new Filling(embargoedId);
            filling.WithdrawnDates.Add(testDate);
            filling.MenuCategories.Add(testCategory);
            repo.SaveFilling(filling);

            var actual = repo.GetAllFillingsAsync(activeOnThisDate: testDate, menuCategoryId: testCategory).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsFalse(actual.Any(f => f.Id == withdrawnId), "mistake: a withdrawn item was returned!");
            Assert.IsFalse(actual.Any(f => f.Id == embargoedId), "mistake: an embargoed item was returned!");
            Assert.IsFalse(actual.Any(f => f.WithdrawnDates.Contains(testDate)), "mistake: an embargoed item (by date) was returned!");
            Assert.AreEqual(2, actual.Count());
        }

        [TestMethod]
        public void Only_available_fillings_for_provider_should_be_returned()
        {
            var baseRepo = new MongoFillingRepository(settings, AsyncRetryPolicy, RetryPolicy);
            var repo = baseRepo as IFillingRepository;

            var testProvider = Guid.NewGuid();
            var withdrawnId = Guid.NewGuid();
            var filling = new Filling(withdrawnId)
            {
                Status = FillingStatus.Withdrawn,
                LunchProviderId =  testProvider 
            };
            repo.SaveFilling(filling);
            filling = new Filling(Guid.NewGuid()) { LunchProviderId =  testProvider };
            repo.SaveFilling(filling);
            filling = new Filling(Guid.NewGuid());
            repo.SaveFilling(filling);
            filling = new Filling(Guid.NewGuid()) { LunchProviderId =  testProvider };
            repo.SaveFilling(filling);
            var testDate = DateTime.Today.AddDays(10).ToUniversalTime();
            var embargoedId = Guid.NewGuid();
            filling = new Filling(embargoedId) { LunchProviderId = testProvider };
            filling.WithdrawnDates.Add(testDate);
            repo.SaveFilling(filling);

            var actual = repo.GetAllFillings(lunchProviderId: testProvider, activeOnThisDate: testDate);

            Assert.IsFalse(actual.Any(f => f.Id == withdrawnId), "mistake: a withdrawn item was returned!");
            Assert.IsFalse(actual.Any(f => f.Id == embargoedId), "mistake: an embargoed item was returned!");
            Assert.IsFalse(actual.Any(f => f.WithdrawnDates.Contains(testDate)), "mistake: an embargoed item (by date) was returned!");
            Assert.AreEqual(2, actual.Count());
        }

        [TestMethod]
        public void Only_available_fillings_for_provider_should_be_returnedAsync()
        {
            var baseRepo = new MongoFillingRepository(settings, AsyncRetryPolicy, RetryPolicy);
            var repo = baseRepo as IFillingRepository;

            var testProvider = Guid.NewGuid();
            var withdrawnId = Guid.NewGuid();
            var filling = new Filling(withdrawnId)
            {
                Status = FillingStatus.Withdrawn,
                LunchProviderId = testProvider 
            };
            repo.SaveFilling(filling);
            filling = new Filling(Guid.NewGuid()) { LunchProviderId = testProvider };
            repo.SaveFilling(filling);
            filling = new Filling(Guid.NewGuid());
            repo.SaveFilling(filling);
            filling = new Filling(Guid.NewGuid()) { LunchProviderId = testProvider };
            repo.SaveFilling(filling);
            var testDate = DateTime.Today.AddDays(10).ToUniversalTime();
            var embargoedId = Guid.NewGuid();
            filling = new Filling(embargoedId)
            {
                LunchProviderId = testProvider
            };
            filling.WithdrawnDates.Add(testDate);
            repo.SaveFilling(filling);

            var actual = repo.GetAllFillingsAsync(activeOnThisDate: testDate, lunchProviderId: testProvider).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsFalse(actual.Any(f => f.Id == withdrawnId), "mistake: a withdrawn item was returned!");
            Assert.IsFalse(actual.Any(f => f.Id == embargoedId), "mistake: an embargoed item was returned!");
            Assert.IsFalse(actual.Any(f => f.WithdrawnDates.Contains(testDate)), "mistake: an embargoed item (by date) was returned!");
            Assert.AreEqual(2, actual.Count());
        }

        [TestMethod]
        public void Only_available_fillings_for_provider_and_category_should_be_returned()
        {
            var baseRepo = new MongoFillingRepository(settings, AsyncRetryPolicy, RetryPolicy);
            var repo = baseRepo as IFillingRepository;

            var testCategory = Guid.NewGuid();
            var testProvider = Guid.NewGuid();
            var withdrawnId = Guid.NewGuid();
            var filling = new Filling(withdrawnId)
            {
                Status = FillingStatus.Withdrawn,
                LunchProviderId = testProvider,
            };
            filling.MenuCategories.Add(testCategory);
            repo.SaveFilling(filling);
            filling = new Filling(Guid.NewGuid());
            filling.MenuCategories.Add(testCategory);
            repo.SaveFilling(filling);
            filling = new Filling(Guid.NewGuid());
            repo.SaveFilling(filling);
            filling = new Filling(Guid.NewGuid())
            {
                LunchProviderId = testProvider
            };
            repo.SaveFilling(filling);
            filling = new Filling(Guid.NewGuid())
            {
                LunchProviderId = testProvider,
            };
            filling.MenuCategories.Add(testCategory);
            repo.SaveFilling(filling);
            filling = new Filling(Guid.NewGuid())
            {
                LunchProviderId = testProvider,
            };
            filling.MenuCategories.Add(testCategory);
            repo.SaveFilling(filling);
            var testDate = DateTime.Today.AddDays(10).ToUniversalTime();
            var embargoedId = Guid.NewGuid();
            filling = new Filling(embargoedId)
            {
                LunchProviderId = testProvider,
            };
            filling.WithdrawnDates.Add(testDate);
            filling.MenuCategories.Add(testCategory);
            repo.SaveFilling(filling);

            var actual = repo.GetAllFillings(lunchProviderId: testProvider, 
                menuCategoryId: testCategory, 
                activeOnThisDate: testDate);

            Assert.IsFalse(actual.Any(f => f.Id == withdrawnId), "mistake: a withdrawn item was returned!");
            Assert.IsFalse(actual.Any(f => f.Id == embargoedId), "mistake: an embargoed item was returned!");
            Assert.IsFalse(actual.Any(f => f.WithdrawnDates.Contains(testDate)), "mistake: an embargoed item (by date) was returned!");
            Assert.AreEqual(2, actual.Count());
        }

        [TestMethod]
        public void Only_available_fillings_for_provider_and_category_should_be_returnedAsync()
        {
            var baseRepo = new MongoFillingRepository(settings, AsyncRetryPolicy, RetryPolicy);
            var repo = baseRepo as IFillingRepository;

            var testCategory = Guid.NewGuid();
            var testProvider = Guid.NewGuid();
            var withdrawnId = Guid.NewGuid();
            var filling = new Filling(withdrawnId)
            {
                Status = FillingStatus.Withdrawn,
                LunchProviderId = testProvider,
            };
            filling.MenuCategories.Add(testCategory);
            repo.SaveFilling(filling);
            filling = new Filling(Guid.NewGuid()) { LunchProviderId = testProvider };
            repo.SaveFilling(filling);
            filling = new Filling(Guid.NewGuid());
            repo.SaveFilling(filling);
            filling = new Filling(Guid.NewGuid());
            filling.MenuCategories.Add(testCategory);
            repo.SaveFilling(filling);
            filling = new Filling(Guid.NewGuid())
            {
                LunchProviderId = testProvider,
            };
            filling.MenuCategories.Add(testCategory);
            repo.SaveFilling(filling);
            filling = new Filling(Guid.NewGuid())
            {
                LunchProviderId = testProvider,
            };
            filling.MenuCategories.Add(testCategory);
            repo.SaveFilling(filling);
            var testDate = DateTime.Today.AddDays(10).ToUniversalTime();
            var embargoedId = Guid.NewGuid();
            filling = new Filling(embargoedId)
            {
                LunchProviderId = testProvider,
            };
            filling.WithdrawnDates.Add(testDate);
            filling.MenuCategories.Add(testCategory);
            repo.SaveFilling(filling);

            var actual = repo.GetAllFillingsAsync(activeOnThisDate: testDate, 
                lunchProviderId: testProvider,
                menuCategoryId: testCategory).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsFalse(actual.Any(f => f.Id == withdrawnId), "mistake: a withdrawn item was returned!");
            Assert.IsFalse(actual.Any(f => f.Id == embargoedId), "mistake: an embargoed item was returned!");
            Assert.IsFalse(actual.Any(f => f.WithdrawnDates.Contains(testDate)), "mistake: an embargoed item (by date) was returned!");
            Assert.AreEqual(2, actual.Count());
        }
    }
}
