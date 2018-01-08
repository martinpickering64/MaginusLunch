using System;
using System.Collections.Generic;

namespace MaginusLunch.Menu.Messages.Events
{
    public class FillingUpdated : MenuEvent
    {
        public FillingUpdated() : this(Guid.Empty, MealType.BakedPotatoe, new Guid[0], string.Empty)
        { }

        public FillingUpdated(Guid fillingId, MealType mealType, IEnumerable<Guid> menuCategories, string editorUserId) 
            : base(fillingId, editorUserId)
        {
            mealType = MealType;
            menuCategories = new List<Guid>(menuCategories);
        }

        public MealType MealType { get; set; }
        public IList<Guid> MenuCategories { get; set; }
    }
}
