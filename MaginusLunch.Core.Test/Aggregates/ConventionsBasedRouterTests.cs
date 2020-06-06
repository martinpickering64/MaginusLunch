using MaginusLunch.Core.Aggregates;
using System;
using Xunit;

namespace MaginusLunch.Core.Test.Aggregates
{
    public class ConventionsBasedRouterTests
    {
        [Fact]
        public void RouterRegistersAllApplyMethods()
        {
            var id = Guid.NewGuid();
            const string name = "test";
            var aggregate = new TestAggregate(id, null);

            var router = new ConventionBasedEventRouter();
            router.Register(aggregate);

            router.Dispatch(new TestCreatedEvent (aggregate.Id, name));
            router.Dispatch(new NameUpdatedEvent (aggregate.Id, name));
        }

        [Fact]
        public void ExceptionForUnregisteredRouteIsThrown()
        {
            var id = Guid.NewGuid();
            var aggregate = new TestAggregate(id, null);

            var router = new ConventionBasedEventRouter();
            router.Register(aggregate);

            Assert.Throws<HandlerForDomainEventNotFoundException>(() => router.Dispatch(new NonExistentEvent(Guid.NewGuid())));
        }

        [Fact]
        public void CantRegisterWithNullAggregate()
        {
            var router = new ConventionBasedEventRouter();

            Assert.Throws<ArgumentNullException>(() => router.Register(null));
        }

        [Fact]
        public void CantDispatchNullEvent()
        {
            var id = Guid.NewGuid();
            var aggregate = new TestAggregate(id, null);

            var router = new ConventionBasedEventRouter();
            router.Register(aggregate);

            Assert.Throws<ArgumentNullException>(() => router.Dispatch(null));
        }

        [Fact]
        public void OnlyRegistersRoutesWithSingleReferenceTypeArg()
        {
            var id = Guid.NewGuid();
            var aggregate = new TestAggregate(id, null);

            var router = new ConventionBasedEventRouter();
            router.Register(aggregate);

            Assert.Throws<HandlerForDomainEventNotFoundException>(() => router.Dispatch(new UnregisteredEvent(Guid.NewGuid())));
        }
    }
}
