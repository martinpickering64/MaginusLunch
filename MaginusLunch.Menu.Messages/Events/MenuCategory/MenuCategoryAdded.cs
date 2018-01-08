using System;

namespace MaginusLunch.Menu.Messages.Events
{
    public class MenuCategoryAdded : MenuEvent
    {
        public MenuCategoryAdded()
            : base()
        { }

        public MenuCategoryAdded(Guid menuCategoryId, string name, string editorUserId)
            : base(menuCategoryId, editorUserId)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}
