using System;

namespace MaginusLunch.Menu.Messages.Events
{
    public class FillingAllowsMenuOptions : MenuEvent
    {
        public FillingAllowsMenuOptions()
            : base()
        { }

        public FillingAllowsMenuOptions(Guid fillingId, string editorUserId)
            : base(fillingId, editorUserId)
        { }
    }
}
