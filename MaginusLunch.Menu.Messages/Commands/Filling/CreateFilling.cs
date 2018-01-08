using System;

namespace MaginusLunch.Menu.Messages.Commands
{
    public class CreateFilling : MenuCommand
    {
        public string Name { get; set; }
        public Guid LunchProviderId { get; set; }
        public MealType MealType { get; set; }
    }
}
