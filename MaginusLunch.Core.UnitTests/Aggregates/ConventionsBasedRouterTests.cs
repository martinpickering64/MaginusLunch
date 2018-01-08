using MaginusLunch.Core.Aggregates;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaginusLunch.Core.UnitTests.Aggregates
{
    [TestCategory("Unit Tests")]
    [TestClass]
    public class ConventionsBasedRouterTests
    {
        [TestMethod]
        public void RouterRegistersAllApplyMethods()
        {
            var id = Guid.NewGuid();
            const string name = "test";
            var aggregate = new TestAggregate(id, null);

            var router = new ConventionBasedEventRouter(aggregate);

            router.Dispatch(new TestCreatedEvent { Id = aggregate.Id, Name = name });
            router.Dispatch(new NameUpdatedEvent { Id = aggregate.Id, Name = name });
        }

        [TestMethod]
        public void RaiseExceptionForUnregisteredRoute()
        {
            var id = Guid.NewGuid();
            var aggregate = new TestAggregate(id, null);

            var router = new ConventionBasedEventRouter(aggregate);

            Assert.ThrowsException<HandlerForDomainEventNotFoundException>(()=>router.Dispatch(new NonExistentEvent()));
        }

        [TestMethod]
        public void Cant_register_with_a_null_aggregate()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ConventionBasedEventRouter(null));
        }

        [TestMethod]
        public void Cant_dispatch_a_null_event()
        {
            var id = Guid.NewGuid();
            var aggregate = new TestAggregate(id, null);

            var router = new ConventionBasedEventRouter(aggregate);

            Assert.ThrowsException<ArgumentNullException>(() => router.Dispatch(null));
        }

        [TestMethod]
        public void Only_registers_routes_with_single_reference_type_arg()
        {
            var id = Guid.NewGuid();
            var aggregate = new TestAggregate(id, null);

            var router = new ConventionBasedEventRouter(aggregate);

            Assert.ThrowsException<HandlerForDomainEventNotFoundException>(() => router.Dispatch(new UnregisteredEvent()));
        }
    }

    public class NonExistentEvent
    { }
}
