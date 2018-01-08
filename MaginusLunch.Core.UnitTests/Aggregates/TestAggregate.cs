using MaginusLunch.Core.Aggregates;
using System;

namespace MaginusLunch.Core.UnitTests.Aggregates
{
    public class TestAggregate : AggregateRoot
    {
        public TestAggregate(Guid id, IRouteEvents handler)
            : base(handler)
        {
            Id = id;
        }

        public string Name { get; set; }

        public TestAggregate(CreateTestCommand command, IRouteEvents handler)
            : this(command.Id, handler)
        {
            Version = -100;
            RaiseEvent(new TestCreatedEvent
            {
                Id = command.Id,
                Name = command.Name
            });
        }

        public void UpdateName(UpdateNameCommand command)
        {
            RaiseEvent(new NameUpdatedEvent
            {
                Id = command.Id,
                Name = command.Name
            });
        }

        public void Apply(TestCreatedEvent theEvent)
        {
            Name = theEvent.Name;
        }

        public void Apply(NameUpdatedEvent theEvent)
        {
            Name = theEvent.Name;
        }

        public void Apply(UnregisteredEvent theEvent, string unexpectedArg)
        {
            throw new NotImplementedException();
        }
    }

    public class CreateTestCommand
    {
        public Guid Id;
        public string Name;
    }

    public class TestCreatedEvent
    {
        public Guid Id;
        public string Name;
    }

    public class UpdateNameCommand
    {
        public Guid Id;
        public string Name;
    }

    public class NameUpdatedEvent
    {
        public Guid Id;
        public string Name;
    }

    public class UnregisteredEvent
    { }
}
