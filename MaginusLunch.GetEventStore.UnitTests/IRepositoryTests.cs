using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using MaginusLunch.Core.EventStore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading.Tasks;

namespace MaginusLunch.GetEventStore.UnitTests
{
    [TestCategory("Unit Tests")]
    [TestClass]
    public class IRepositoryTests
    {
        [TestMethod]
        public void Cant_get_aggregate_with_version_less_than_zero()
        {
            var gesConnection = new Mock<IEventStoreConnection>();
            var factory = new Mock<IConstructAggregates>();
            var repository = new GetEventStoreRepository(eventStoreConnection: gesConnection.Object,
                aggregateFactory: factory.Object);

            Assert.ThrowsException<InvalidOperationException>(() => repository.GetById<TestAggregate>(Guid.NewGuid(), -1));
        }

        [TestMethod]
        public void Verify_stream_name()
        {
            const string expected = "testAggregate-1d8ea2e9dd5c483291bc8abab1502273";
            string testId = "1d8ea2e9dd5c483291bc8abab1502273";
            Func<Type, Guid, string> strategy = (type, guid) => 
                $"{char.ToLowerInvariant(type.Name[0])}{type.Name.Substring(1)}-{guid.ToString("N")}";

            Assert.AreEqual(expected, strategy(typeof(TestAggregate), new Guid(testId)));
        }

        //[TestMethod]
        //public void When_stream_is_not_found()
        //{
        //    var testAggregate = new TestAggregate(Guid.NewGuid());
        //    var streamTask = new Mock<Task<StreamEventsSlice>>();
        //    var streamEventSlice = new Mock<StreamEventsSlice>();
        //    streamEventSlice.SetupProperty(x => x.Status, SliceReadStatus.StreamNotFound);
        //    streamTask.SetupProperty(x => x.Result, streamEventSlice.Object);
        //    var gesConnection = new Mock<IEventStoreConnection>();
        //    gesConnection.Setup(x => x.ReadStreamEventsForwardAsync(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<int>(), It.IsAny<bool>(), null))
        //        .Returns(() => streamTask.Object);
        //    var factory = new Mock<IConstructAggregates>();
        //    factory.Setup(x => x.Build(typeof(TestAggregate), testAggregate.Id)).Returns(testAggregate);
        //    var repository = new GetEventStoreRepository(eventStoreConnection: gesConnection.Object,
        //        aggregateFactory: factory.Object,
        //        publishOnTheBus: (anEvent) => Task.Delay(1));

        //    Assert.ThrowsException<InvalidOperationException>(() => repository.GetById<TestAggregate>(testAggregate.Id));
        //}
    }
}
