using System;

namespace MaginusLunch.Menu.Messages.Commands
{
    public class CreateMenuOption : MenuCommand
    {
        public string Name { get; set; }
        public Guid LunchProviderId { get; set; }
    }
}
