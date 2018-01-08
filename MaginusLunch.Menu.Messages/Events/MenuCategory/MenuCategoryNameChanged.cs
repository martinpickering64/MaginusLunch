using System;

namespace MaginusLunch.Menu.Messages.Events
{
    public class MenuCategoryNameChanged : MenuEvent
    {
        public MenuCategoryNameChanged()
            : base()
        { }

        public MenuCategoryNameChanged(Guid menuCategoryId, string newName, string editoUserId)
            : base(menuCategoryId, editoUserId)
        {
            NewName = newName;
        }

        public string NewName { get; set; }
    }
}
