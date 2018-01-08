using System;
using MaginusLunch.Core.Aggregates;

namespace MaginusLunch.Core.EventStore
{
    public class DefaultAggregateFactory : IConstructAggregates
    {
        public IAggregate Build(Type type, Guid id)
        {
            return (IAggregate)Activator.CreateInstance(type, id);
        }
    }
}
