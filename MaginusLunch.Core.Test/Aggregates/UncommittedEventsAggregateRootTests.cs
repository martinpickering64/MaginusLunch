using Xunit;
using MaginusLunch.Core.Aggregates;
using System;
using System.Linq;

namespace MaginusLunch.Core.Test.Aggregates
{
    public class UncommittedEventsAggregateRootTests
    {
        private static TestAggregate Given_a_TestAggregate_is_created(Guid id, string name, IRouteEvents handler)
        {
            return new TestAggregate(new CreateTestCommand (id, name), handler);
        }

        private static void And_the_name_of_the_TestAggregate_is_updated(TestAggregate aggregate, string newName)
        {
            var command = new UpdateNameCommand(aggregate.Id, newName);
            aggregate.UpdateName(command);
        }

        [Fact]
        public void UncommittedEventsIsEmpty()
        {
            var aggregate = new TestAggregate(Guid.NewGuid(), null);

            Assert.Empty(aggregate.UncommittedEvents);
        }

        [Fact]
        public void UncommittedEventsAcquiresEvents()
        {
            var aggregate = Given_a_TestAggregate_is_created(Guid.NewGuid(), "test", null);
            And_the_name_of_the_TestAggregate_is_updated(aggregate, "two");
            And_the_name_of_the_TestAggregate_is_updated(aggregate, "three");
            And_the_name_of_the_TestAggregate_is_updated(aggregate, "four");
            And_the_name_of_the_TestAggregate_is_updated(aggregate, "five");

            Assert.Equal(5, aggregate.UncommittedEvents.Count());
        }

        [Fact]
        public void UncommittedEventsCanBeClearedDown()
        {
            var aggregate = Given_a_TestAggregate_is_created(Guid.NewGuid(), "test", null);
            And_the_name_of_the_TestAggregate_is_updated(aggregate, "two");
            And_the_name_of_the_TestAggregate_is_updated(aggregate, "three");
            And_the_name_of_the_TestAggregate_is_updated(aggregate, "four");
            And_the_name_of_the_TestAggregate_is_updated(aggregate, "five");

            aggregate.ClearUncommittedEvents();

            Assert.Empty(aggregate.UncommittedEvents);
        }
    }
}
