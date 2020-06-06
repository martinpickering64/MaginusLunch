using Xunit;
using MaginusLunch.Core.Aggregates;
using Moq;
using System;
using System.Linq;
using System.Diagnostics;

namespace MaginusLunch.Core.Test.Aggregates
{
    public class AmendAggregateRootTests
    {
        private static TestAggregate Given_a_TestAggregate_is_created(Guid id, string name, IRouteEvents handler)
        {
            return new TestAggregate(new CreateTestCommand (id, name ), handler);
        }

        private static void When_the_name_of_the_TestAggregate_is_updated(TestAggregate aggregate, string newName)
        {
            var command = new UpdateNameCommand(aggregate.Id, newName);
            aggregate.UpdateName(command);
        }

        [Fact]
        public void NameUpdatedEventIsRaised()
        {
            const string name = "test";
            const string newName = "amended";
            var id = Guid.NewGuid();
            var router = new Mock<IRouteEvents>();
            var routerObj = router.Object;

            var aggregate = Given_a_TestAggregate_is_created(id, name, routerObj);

            When_the_name_of_the_TestAggregate_is_updated(aggregate, newName);

            router.Verify(x => x.Dispatch(It.IsAny<NameUpdatedEvent>()), Times.Once);
            var events = aggregate.UncommittedEvents;
            Debug.Print("After: var events = aggregate.UncommittedEvents;");
            Assert.Equal(2, events.Count());
            var lastEvent = events.Last();
            Assert.IsType<NameUpdatedEvent>(lastEvent);
        }

        [Fact]
        public void NameUpdatedEventCarriesTheCorrectId()
        {
            const string name = "test";
            const string newName = "amended";
            var id = Guid.NewGuid();

            var aggregate = Given_a_TestAggregate_is_created(id, name, null);

            When_the_name_of_the_TestAggregate_is_updated(aggregate, newName);

            var events = aggregate.UncommittedEvents;
            var lastEvent = events.Last() as NameUpdatedEvent;
            Assert.Equal(id, lastEvent.Id);
        }

        [Fact]
        public void NameUpdatedEventCarriesTheCorrectName()
        {
            const string name = "test";
            const string newName = "amended";
            var id = Guid.NewGuid();

            var aggregate = Given_a_TestAggregate_is_created(id, name, null);

            When_the_name_of_the_TestAggregate_is_updated(aggregate, newName);

            var events = aggregate.UncommittedEvents;
            var lastEvent = events.Last() as NameUpdatedEvent;
            Assert.Equal(newName, lastEvent.Name);
        }

        [Fact]
        public void TheAggregateVersionIsIncremented()
        {
            const string name = "test";
            const string newName = "amended";
            var id = Guid.NewGuid();

            var aggregate = Given_a_TestAggregate_is_created(id, name, null);
            var initialVersion = aggregate.Version;

            When_the_name_of_the_TestAggregate_is_updated(aggregate, newName);

            Assert.Equal(initialVersion + 1, aggregate.Version);

        }

        [Fact]
        public void TheAggregateStateIsCorrect()
        {
            const string name = "test";
            const string newName = "amended";
            var id = Guid.NewGuid();

            var aggregate = Given_a_TestAggregate_is_created(id, name, null);

            When_the_name_of_the_TestAggregate_is_updated(aggregate, newName);

            Assert.Equal(id, aggregate.Id);
            Assert.Equal(newName, aggregate.Name);
        }
    }
}
