using System;

namespace MaginusLunch.Menu.Messages.Events
{
    public class MenuOptionActivated : MenuEvent
    {
        public MenuOptionActivated()
            : base()
        { }

        public MenuOptionActivated(Guid menuOptionId, string editorUserId)
            : base(menuOptionId, editorUserId)
        { }
    }
}
