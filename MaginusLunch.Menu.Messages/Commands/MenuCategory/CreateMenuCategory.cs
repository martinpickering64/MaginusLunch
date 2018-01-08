using System;

namespace MaginusLunch.Menu.Messages.Commands
{
    public class CreateMenuCategory : MenuCommand
    {
        public string Name { get; set; }
        public Guid LunchProviderId { get; set; }
    }
}
