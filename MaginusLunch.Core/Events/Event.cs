using System;

namespace MaginusLunch.Core.Events
{
    public abstract class Event
    {
        protected Event(Guid id)
        {
            if (Guid.Empty.Equals(id)) throw new ArgumentException("Cannot be empty.", nameof(id));
            Id = id;
        }

        public Guid Id { get; }
    }
}
