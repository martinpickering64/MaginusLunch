using System;

namespace MaginusLunch.Core.Test.Aggregates
{
    public class CreateTestCommand : Commands.Command
    {
        public CreateTestCommand(Guid id, string name):base(id)
        {
            Name = name;
        }

        public string Name { get; }
    }
}
