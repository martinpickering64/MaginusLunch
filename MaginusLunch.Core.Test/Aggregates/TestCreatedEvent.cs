using System;

namespace MaginusLunch.Core.Test.Aggregates
{
    public class TestCreatedEvent : Events.Event
    {
        public TestCreatedEvent(Guid id, string name) : base(id)
        {
            Name = name;
        }

        public string Name { get; }
    }
}
