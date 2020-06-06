using MaginusLunch.Core.Aggregates;
using System;

namespace MaginusLunch.Core.Test.Aggregates
{
    public class TestAggregate : AggregateRoot
    {
        public TestAggregate(Guid id, IRouteEvents handler)
            : base(handler)
        {
            if (id.Equals(Guid.Empty)) throw new ArgumentException("Must not be empty.", nameof(id));
            Id = id;
        }

        public string Name { get; set; }

        public TestAggregate(CreateTestCommand command, IRouteEvents handler)
            : this(command is null ? Guid.Empty : command.Id, handler)
        {
            if (command is null) throw new ArgumentNullException(nameof(command));
            Version = -100;
            RaiseEvent(new TestCreatedEvent(command.Id, command.Name));
        }

        public void UpdateName(UpdateNameCommand command)
        {
            if (command is null) throw new ArgumentNullException(nameof(command));
            RaiseEvent(new NameUpdatedEvent(command.Id, command.Name));
        }

        public void Apply(TestCreatedEvent theEvent)
        {
            if (theEvent is null) throw new ArgumentNullException(nameof(theEvent));
            Name = theEvent.Name;
        }

        public void Apply(NameUpdatedEvent theEvent)
        {
            if (theEvent is null) throw new ArgumentNullException(nameof(theEvent));
            Name = theEvent.Name;
        }

        public void Apply(UnregisteredEvent theEvent, string unexpectedArg)
        {
            throw new NotImplementedException();
        }
    }
}
