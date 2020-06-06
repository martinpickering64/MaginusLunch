using Xunit;
using MaginusLunch.Core.Aggregates;
using Moq;
using System;
using System.Linq;

namespace MaginusLunch.Core.Test.Aggregates
{
    public class CreateAggregateRootTests
    {
        private static TestAggregate When_a_TestAggregate_is_created(Guid id, string name, IRouteEvents handler)
        {
            return new TestAggregate(new CreateTestCommand(id, name), handler);
        }

        [Fact]
        public void TestAggregateCreatedEventIsRaised()
        {
            var router = new Mock<IRouteEvents>();

            var aggregate = When_a_TestAggregate_is_created(Guid.NewGuid(), "test", router.Object);

            router.Verify(x => x.Dispatch(It.IsAny<TestCreatedEvent>()), Times.Once);
            var events = aggregate.UncommittedEvents;
            Assert.Single(events);
            var firstEvent = events.First();
            Assert.IsType<TestCreatedEvent>(firstEvent);
        }

        [Fact]
        public void TestAggregateCreatedEventCarriesTheCorrectId()
        {
            var id = Guid.NewGuid();
            var aggregate = When_a_TestAggregate_is_created(id, "test", null);

            var events = aggregate.UncommittedEvents;
            var firstEvent = events.First() as TestCreatedEvent;
            Assert.Equal(id, firstEvent.Id);
        }

        [Fact]
        public void TestAggregateCreatedEventCarriesTheCorrectName()
        {
            const string name = "test";
            var id = Guid.NewGuid();
            var aggregate = When_a_TestAggregate_is_created(id, name, null);

            var events = aggregate.UncommittedEvents;
            var firstEvent = events.First() as TestCreatedEvent;
            Assert.Equal(name, firstEvent.Name);
        }

        [Fact]
        public void TheAggregateVersionIsIncremented()
        {
            var id = Guid.NewGuid();
            var aggregate = When_a_TestAggregate_is_created(id, "test", null);

            Assert.Equal(-99, aggregate.Version);
        }

        [Fact]
        public void TheAggregateStateIsCorrect()
        {
            const string name = "test";
            var id = Guid.NewGuid();
            var aggregate = When_a_TestAggregate_is_created(id, name, null);

            Assert.Equal(id, aggregate.Id);
            Assert.Equal(name, aggregate.Name);
        }
    }
}
