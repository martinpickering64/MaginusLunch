using System;

namespace MaginusLunch.Core.Test.Aggregates
{
    public class NameUpdatedEvent: Events.Event
    {
        public NameUpdatedEvent(Guid id, string name):base(id)
        {
            Name = name;
        }

        public string Name { get; }
    }
}
