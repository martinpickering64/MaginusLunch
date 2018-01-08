using System;
using System.Collections.Generic;

namespace MaginusLunch.Menu.Messages.Commands
{
    public class UpdateFilling : MenuCommand
    {
        public MealType MealType { get; set; }
        public IList<Guid> MenuCategories { get; set; }
    }
}
