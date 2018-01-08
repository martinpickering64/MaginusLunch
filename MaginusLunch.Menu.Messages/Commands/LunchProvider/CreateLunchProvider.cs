using System;

namespace MaginusLunch.Menu.Messages.Commands
{
    public class CreateLunchProvider : MenuCommand
    {
        public string Name { get; set; }
        public Guid LunchProviderId { get; set; }
    }
}
