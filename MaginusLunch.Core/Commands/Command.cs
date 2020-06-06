using System;

namespace MaginusLunch.Core.Commands
{
    public abstract class Command
    {
        protected Command (Guid id)
        {
            if (Guid.Empty.Equals(id)) throw new ArgumentException("Cannot be empty.", nameof(id));
            Id = id;
        }

        public Guid Id { get; }
    }
}
