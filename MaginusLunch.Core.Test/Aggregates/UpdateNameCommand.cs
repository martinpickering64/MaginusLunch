using System;

namespace MaginusLunch.Core.Test.Aggregates
{
    public class UpdateNameCommand : Commands.Command
    {
        public UpdateNameCommand(Guid id, string name) : base(id)
        {
            Name = name;
        }

        public string Name { get; }
    }
}
