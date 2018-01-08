using System;

namespace MaginusLunch.Menu.Messages.Events
{
    public class FillingActivated : MenuEvent
    {
        public FillingActivated()
            : base()
        { }

        public FillingActivated(Guid fillingId, string editorUserId)
            : base(fillingId, editorUserId)
        { }
    }
}
