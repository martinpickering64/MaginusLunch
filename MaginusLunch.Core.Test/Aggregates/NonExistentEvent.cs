using System;

namespace MaginusLunch.Core.Test.Aggregates
{
    public class NonExistentEvent : Events.Event
    {
        public NonExistentEvent(Guid id) : base(id)
        {  }
    }
}
