using MaginusLunch.Core.EventStore;
using MaginusLunch.GetEventStore;
using MaginusLunch.Orders.Domain;
using MaginusLunch.Orders.Messages.Commands;
using MaginusLunch.Orders.Messages.Events;
using MaginusLunch.Orders.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using MaginusLunch.Logging;
using EventStore.ClientAPI;
using GES = EventStore.ClientAPI;
using System.Diagnostics;

namespace MaginusLunch.Orders.Denormalizer.Console.IntegrationTests.EventStore
{
    [TestCategory("Integration tests")]
    [TestClass]
    public class TestWithEventStore
    {
        const string SubscriptionStream = "MaginusLunch-Orders";
        const string SubscriptionGroup = "IntegrationTest";

        [TestMethod]
        public void When_a_command_is_added_events_are_captured()
        {

            var addOrder = new AddOrder
            {
                Id = Guid.NewGuid(),
                RecipientUserId = "martin",
                DeliveryDate = GetDeliveryDate()
            };
            var logFactoryInstance = LogManager.Use<DefaultFactory>();
            logFactoryInstance.Level(LogLevel.Debug);
            var eventStoreConnection = Program.CreateEventStoreConnection();
            CleanUpThePersistentSubscription(SubscriptionStream, SubscriptionGroup, eventStoreConnection);
            var gesRepository = new GetEventStoreRepository(eventStoreConnection, new DefaultAggregateFactory());
            CreateFakeRepositories();
            var eventHandlers = new Mock<OrderEventHandlers>(_orderRepository.Object,
                _lunchProviderRepository.Object,
                _fillingRepository.Object,
                _breadRepository.Object,
                _menuOptionRepository.Object,
                _calendarRepository.Object,
                gesRepository);

            var testTask = new DenormalizerServiceWithToken(
                eventHandlers.Object, 
                eventStoreConnection, 
                GetServiceSettings());
            Program.TheTask = testTask;

            AddOrder(addOrder, gesRepository);


            Program.Start();

            Thread.Sleep(5000);

            Program.Stop();
//            var token = testTask.TokenSrc.Token;
            eventHandlers.Verify(e => e.HandleAsync(
                                            It.IsAny<OrderAdded>(), 
                                            It.IsAny<CancellationToken>()), 
                                            Times.Once());
        }

        [TestMethod]
        public void When_the_subscription_is_inaccessible()
        {
            var logFactoryInstance = LogManager.Use<DefaultFactory>();
            logFactoryInstance.Level(LogLevel.Debug);
            var eventStoreConnection = Program.CreateEventStoreConnection();
            var gesRepository = new GetEventStoreRepository(eventStoreConnection, new DefaultAggregateFactory());
            CleanUpThePersistentSubscription(SubscriptionStream, SubscriptionGroup, eventStoreConnection);
            CreateFakeRepositories();
            var eventHandlers = new Mock<OrderEventHandlers>(_orderRepository.Object,
                _lunchProviderRepository.Object,
                _fillingRepository.Object,
                _breadRepository.Object,
                _menuOptionRepository.Object,
                _calendarRepository.Object,
                gesRepository);
            var serviceSettings = GetServiceSettings();
            serviceSettings.GroupName = SubscriptionGroup + "Absent";

            var testTask = new DenormalizerServiceWithToken(
                eventHandlers.Object,
                eventStoreConnection,
                serviceSettings);
            Program.TheTask = testTask;
            
            Assert.IsFalse(Program.Start());
        }

        [TestMethod]
        public void When_consuming_an_event_there_is_a_failure()
        {
            var addOrder = new AddOrder
            {
                Id = Guid.NewGuid(),
                RecipientUserId = "martin",
                DeliveryDate = GetDeliveryDate()
            };
            var logFactoryInstance = LogManager.Use<DefaultFactory>();
            logFactoryInstance.Level(LogLevel.Debug);
            var eventStoreConnection = Program.CreateEventStoreConnection();
            var gesRepository = new GetEventStoreRepository(eventStoreConnection, new DefaultAggregateFactory());
            CleanUpThePersistentSubscription(SubscriptionStream, SubscriptionGroup, eventStoreConnection);
            CreateFakeRepositories();
            var eventHandlers = new Mock<OrderEventHandlers>(_orderRepository.Object,
                _lunchProviderRepository.Object,
                _fillingRepository.Object,
                _breadRepository.Object,
                _menuOptionRepository.Object,
                _calendarRepository.Object,
                gesRepository);
            eventHandlers.Setup(eh => eh.HandleAsync(It.IsAny<OrderAdded>(),
                                            It.IsAny<CancellationToken>())).ThrowsAsync(new InvalidOperationException("A Test Exception"));

            var testTask = new DenormalizerServiceWithToken(
                eventHandlers.Object,
                eventStoreConnection,
                GetServiceSettings(),
                (sub, reason, ex) => {
                            _subscriptionDroppedReason = reason;
                            _subscriptionDroppedException = ex;
                        }
                );
            Program.TheTask = testTask;
            AddOrder(addOrder, gesRepository);

            Assert.IsTrue(Program.Start());
            Thread.Sleep(5000);
            Program.Stop();

            Assert.AreEqual(SubscriptionDropReason.EventHandlerException, _subscriptionDroppedReason);
        }

        private SubscriptionDropReason _subscriptionDroppedReason;
        private Exception _subscriptionDroppedException;

        private void AddOrder(AddOrder command, IRepository eventStoreRepo)
        {
            var orderAggregate = new OrderAggregate(command);
            eventStoreRepo.Save(orderAggregate, Guid.NewGuid());
        }

        Mock<IOrderRepository> _orderRepository;
        Mock<ILunchProviderRepository> _lunchProviderRepository;
        Mock<IFillingRepository> _fillingRepository;
        Mock<IBreadRepository> _breadRepository;
        Mock<IMenuOptionRepository> _menuOptionRepository;
        Mock<ICalendarRepository> _calendarRepository;

        DateTime GetDeliveryDate()
        {
            var tomorrow = DateTime.UtcNow.Date.AddDays(1);
            if (tomorrow.DayOfWeek == DayOfWeek.Saturday)
            {
                tomorrow = tomorrow.AddDays(2);
            }
            if (tomorrow.DayOfWeek == DayOfWeek.Sunday)
            {
                tomorrow = tomorrow.AddDays(1);
            }
            return tomorrow;
        }
        private void CreateFakeRepositories()
        {
            _orderRepository = new Mock<IOrderRepository>();
            _lunchProviderRepository = new Mock<ILunchProviderRepository>();
            _fillingRepository = new Mock<IFillingRepository>();
            _breadRepository = new Mock<IBreadRepository>();
            _menuOptionRepository = new Mock<IMenuOptionRepository>();
            _calendarRepository = new Mock<ICalendarRepository>();
        }

        private void CleanUpThePersistentSubscription(
            string subscriptionStream,
            string subscriptionGroup,
            IEventStoreConnection eventStoreConnection)
        {
            var credentials = new GES.SystemData.UserCredentials("admin", "changeit");
            var settings = PersistentSubscriptionSettings.Create()
                .StartFromCurrent()
                .WithMaxRetriesOf(10)
                .WithMessageTimeoutOf(TimeSpan.FromMilliseconds(10000))
                .WithLiveBufferSizeOf(500)
                .WithBufferSizeOf(500)
                .WithReadBatchOf(20)
                .CheckPointAfter(TimeSpan.FromMilliseconds(1000))
                .MinimumCheckPointCountOf(10)
                .MaximumCheckPointCountOf(500)
                .WithMaxSubscriberCountOf(100)
                .PreferDispatchToSingle()
                .ResolveLinkTos();
            try
            {
                eventStoreConnection.DeletePersistentSubscriptionAsync(subscriptionStream, subscriptionGroup)
                    .ConfigureAwait(false).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Debug.Print("Failed to delete Persistent Subscription for stream {0} and group {1}. Reason '{2}'.",
                    subscriptionStream, subscriptionGroup, ex.Message);
            }
            eventStoreConnection.CreatePersistentSubscriptionAsync(
                subscriptionStream, subscriptionGroup,
                settings,
                credentials)
                .ConfigureAwait(false).GetAwaiter().GetResult();
        }

        private DenormalizerServiceSettings GetServiceSettings()
        {
            return new DenormalizerServiceSettings
            {
                StreamName = SubscriptionStream,
                GroupName = SubscriptionGroup
            };
        }
    }
}
