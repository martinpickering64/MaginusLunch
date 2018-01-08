using System;

namespace MaginusLunch.Menu.Messages.Events
{
    public class MenuOptionNameChanged : MenuEvent
    {
        public MenuOptionNameChanged() 
            : base()
        { }

        public MenuOptionNameChanged(Guid menuOptionId, string newName, string editorUserId) 
            : base(menuOptionId, editorUserId)
        {
            Name = newName;
        }

        public string Name { get; set; }
    }
}
