using System;

namespace MaginusLunch.Menu.Messages.Events
{
    public class FillingDisallowsBread : MenuEvent
    {
        public FillingDisallowsBread()
            : base()
        { }

        public FillingDisallowsBread(Guid fillingId, string editorUserId)
            : base(fillingId, editorUserId)
        { }
    }
}
