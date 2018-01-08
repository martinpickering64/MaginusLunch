using System;

namespace MaginusLunch.Menu.Messages.Events
{
    public class FillingDisallowsMenuOptions : MenuEvent
    {
        public FillingDisallowsMenuOptions()
            : base()
        { }

        public FillingDisallowsMenuOptions(Guid fillingId, string editorUserId)
            : base(fillingId, editorUserId)
        { }
    }
}
