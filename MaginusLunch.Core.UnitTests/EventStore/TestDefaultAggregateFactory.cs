using MaginusLunch.Core.Aggregates;
using MaginusLunch.Core.EventStore;
using MaginusLunch.Core.UnitTests.Aggregates;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaginusLunch.Core.UnitTests.EventStore
{
    [TestCategory("Unit Tests")]
    [TestClass]
    public class TestDefaultAggregateFactory
    {
        [TestMethod]
        public void Will_build_an_aggregateRoot()
        {
            var id = Guid.NewGuid();

            var factory = new DefaultAggregateFactory();
            var aggregate = factory.Build(typeof(BuildableAggregate), id);

            Assert.IsNotNull(aggregate);
            Assert.AreEqual(id, aggregate.Id);
        }

        [TestMethod]
        public void Only_builds_if_the_right_constructor_is_available()
        {
            var id = Guid.NewGuid();

            var factory = new DefaultAggregateFactory();
            Assert.ThrowsException<MissingMethodException>(()=> factory.Build(typeof(UnBuildableAggregate), id));
        }
    }

    public class UnBuildableAggregate : AggregateRoot
    {
        public UnBuildableAggregate(string aString)
        { }
    }
    public class BuildableAggregate : AggregateRoot
    {
        public BuildableAggregate(Guid id)
        {
            Id = id;
        }
    }
}