using System;

namespace MaginusLunch.Core.Test.Aggregates
{
    public class UnregisteredEvent : Events.Event
    {
        public UnregisteredEvent(Guid id) : base(id)
        {  }
    }
}
