using System;

namespace MaginusLunch.Menu.Messages.Events
{
    public class FillingAllowsBread : MenuEvent
    {
        public FillingAllowsBread()
            : base()
        { }

        public FillingAllowsBread(Guid fillingId, string editorUserId)
            : base(fillingId, editorUserId)
        { }
    }
}
