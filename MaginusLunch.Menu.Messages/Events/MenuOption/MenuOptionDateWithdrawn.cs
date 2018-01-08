using System;

namespace MaginusLunch.Menu.Messages.Events
{
    public class MenuOptionDateWithdrawn : MenuEvent
    {
        public MenuOptionDateWithdrawn()
            : base()
        {}

        public MenuOptionDateWithdrawn(Guid MenuOptionId, DateTime affectedDate, string editorUserId)
            : base(MenuOptionId, editorUserId)
        {
            AffectedDate = affectedDate;
        }

        public DateTime AffectedDate { get; set; }
    }
}
