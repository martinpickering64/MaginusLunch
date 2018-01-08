using MaginusLunch.Core.Aggregates;
using System;

namespace MaginusLunch.Core.EventStore
{
    public interface IConstructAggregates
    {
        IAggregate Build(Type type, Guid id);
    }
}
