using MaginusLunch.Core.Aggregates;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Linq;

namespace MaginusLunch.Core.UnitTests.Aggregates
{
    [TestCategory("Unit Tests")]
    [TestClass]
    public class CreateAggrgeateRootTests
    {
        private TestAggregate When_a_TestAggregate_is_created(Guid id, string name, IRouteEvents handler)
        {
            return new TestAggregate(new CreateTestCommand { Id = id, Name = name }, handler);
        }

        [TestMethod]
        public void TestAggregateCreated_event_is_raised()
        {
            var router = new Mock<IRouteEvents>();

            var aggregate = When_a_TestAggregate_is_created(Guid.NewGuid(), "test", router.Object);

            router.Verify(x => x.Dispatch(It.IsAny<TestCreatedEvent>()), Times.Once);
            Assert.AreEqual(1, aggregate.GetUncommittedEvents().Count);
            var events = aggregate.GetUncommittedEvents();
            var firstEvent = events.First();
            Assert.IsInstanceOfType(firstEvent, typeof(TestCreatedEvent));
        }

        [TestMethod]
        public void TestAggregateCreated_event_carries_the_correct_Id()
        {
            var id = Guid.NewGuid();
            var aggregate = When_a_TestAggregate_is_created(id, "test", null);

            var events = aggregate.GetUncommittedEvents();
            var firstEvent = events.First() as TestCreatedEvent;
            Assert.AreEqual(id, firstEvent.Id);
        }

        [TestMethod]
        public void TestAggregateCreated_event_carries_the_correct_Name()
        {
            const string name = "test";
            var id = Guid.NewGuid();
            var aggregate = When_a_TestAggregate_is_created(id, name, null);

            var events = aggregate.GetUncommittedEvents();
            var firstEvent = events.First() as TestCreatedEvent;
            Assert.AreEqual(name, firstEvent.Name);
        }

        [TestMethod]
        public void The_aggregate_version_is_incremented()
        {
            var id = Guid.NewGuid();
            var aggregate = When_a_TestAggregate_is_created(id, "test", null);

            Assert.AreEqual(-99, aggregate.Version);
        }

        [TestMethod]
        public void The_aggregate_state_is_correct()
        {
            const string name = "test";
            var id = Guid.NewGuid();
            var aggregate = When_a_TestAggregate_is_created(id, name, null);

            Assert.AreEqual(id, aggregate.Id);
            Assert.AreEqual(name, aggregate.Name);
        }
    }

    [TestCategory("Unit Tests")]
    [TestClass]
    public class AmendAggrgeateRootTests
    {
        private TestAggregate Given_a_TestAggregate_is_created(Guid id, string name, IRouteEvents handler)
        {
            return new TestAggregate(new CreateTestCommand { Id = id, Name = name }, handler);
        }

        private void When_the_name_of_the_TestAggregate_is_updated(TestAggregate aggregate, string newName)
        {
            var command = new UpdateNameCommand
            {
                Id = aggregate.Id,
                Name = newName
            };
            aggregate.UpdateName(command);

        }

        [TestMethod]
        public void NameUpdated_event_is_raised()
        {
            const string name = "test";
            const string newName = "amended";
            var id = Guid.NewGuid();
            var router = new Mock<IRouteEvents>();

            var aggregate = Given_a_TestAggregate_is_created(id, name, router.Object);

            When_the_name_of_the_TestAggregate_is_updated(aggregate, newName);

            router.Verify(x => x.Dispatch(It.IsAny<NameUpdatedEvent>()), Times.Once);
            Assert.AreEqual(2, aggregate.GetUncommittedEvents().Count);
            var events = aggregate.GetUncommittedEvents();
            var lastEvent = events.Last();
            Assert.IsInstanceOfType(lastEvent, typeof(NameUpdatedEvent));
        }

        [TestMethod]
        public void NameUpdated_event_carries_the_correct_Id()
        {
            const string name = "test";
            const string newName = "amended";
            var id = Guid.NewGuid();

            var aggregate = Given_a_TestAggregate_is_created(id, name, null);

            When_the_name_of_the_TestAggregate_is_updated(aggregate, newName);

            var events = aggregate.GetUncommittedEvents();
            var lastEvent = events.Last() as NameUpdatedEvent;
            Assert.AreEqual(id, lastEvent.Id);
        }

        [TestMethod]
        public void NameUpdated_event_carries_the_correct_Name()
        {
            const string name = "test";
            const string newName = "amended";
            var id = Guid.NewGuid();

            var aggregate = Given_a_TestAggregate_is_created(id, name, null);

            When_the_name_of_the_TestAggregate_is_updated(aggregate, newName);

            var events = aggregate.GetUncommittedEvents();
            var lastEvent = events.Last() as NameUpdatedEvent;
            Assert.AreEqual(newName, lastEvent.Name);
        }

        [TestMethod]
        public void The_aggregate_version_is_incremented()
        {
            const string name = "test";
            const string newName = "amended";
            var id = Guid.NewGuid();

            var aggregate = Given_a_TestAggregate_is_created(id, name, null);
            var initialVersion = aggregate.Version;

            When_the_name_of_the_TestAggregate_is_updated(aggregate, newName);

            Assert.AreEqual(initialVersion + 1, aggregate.Version);

        }

        [TestMethod]
        public void The_aggregate_state_is_correct()
        {
            const string name = "test";
            const string newName = "amended";
            var id = Guid.NewGuid();

            var aggregate = Given_a_TestAggregate_is_created(id, name, null);

            When_the_name_of_the_TestAggregate_is_updated(aggregate, newName);

            Assert.AreEqual(id, aggregate.Id);
            Assert.AreEqual(newName, aggregate.Name);
        }
    }
    [TestCategory("Unit Tests")]
    [TestClass]
    public class UncommittedEventsAggrgeateRootTests
    {
        private TestAggregate Given_a_TestAggregate_is_created(Guid id, string name, IRouteEvents handler)
        {
            return new TestAggregate(new CreateTestCommand { Id = id, Name = name }, handler);
        }

        private void And_the_name_of_the_TestAggregate_is_updated(TestAggregate aggregate, string newName)
        {
            var command = new UpdateNameCommand
            {
                Id = aggregate.Id,
                Name = newName
            };
            aggregate.UpdateName(command);

        }
        [TestMethod]
        public void UncommittedEvents_is_empty()
        {
            var aggregate = new TestAggregate(Guid.NewGuid(), null);

            Assert.AreEqual(0, aggregate.GetUncommittedEvents().Count);
        }

        [TestMethod]
        public void UncommittedEvents_acquires_events()
        {
            var aggregate = Given_a_TestAggregate_is_created(Guid.NewGuid(), "test", null);
            And_the_name_of_the_TestAggregate_is_updated(aggregate, "two");
            And_the_name_of_the_TestAggregate_is_updated(aggregate, "three");
            And_the_name_of_the_TestAggregate_is_updated(aggregate, "four");
            And_the_name_of_the_TestAggregate_is_updated(aggregate, "five");

            Assert.AreEqual(5, aggregate.GetUncommittedEvents().Count);
        }

        [TestMethod]
        public void UncommittedEvents_can_be_cleared_down()
        {
            var aggregate = Given_a_TestAggregate_is_created(Guid.NewGuid(), "test", null);
            And_the_name_of_the_TestAggregate_is_updated(aggregate, "two");
            And_the_name_of_the_TestAggregate_is_updated(aggregate, "three");
            And_the_name_of_the_TestAggregate_is_updated(aggregate, "four");
            And_the_name_of_the_TestAggregate_is_updated(aggregate, "five");

            aggregate.ClearUncommittedEvents();

            Assert.AreEqual(0, aggregate.GetUncommittedEvents().Count);
        }
    }
}
