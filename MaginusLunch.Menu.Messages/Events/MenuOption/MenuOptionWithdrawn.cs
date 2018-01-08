using System;

namespace MaginusLunch.Menu.Messages.Events
{
    public class MenuOptionWithdrawn : MenuEvent
    {
        public MenuOptionWithdrawn()
            : base()
        { }

        public MenuOptionWithdrawn(Guid menuOptionId, string editorUserId)
            : base(menuOptionId, editorUserId)
        { }
    }
}
