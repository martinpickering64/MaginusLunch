using System;

namespace MaginusLunch.Menu.Messages.Events
{
    public class FillingWithdrawn : MenuEvent
    {
        public FillingWithdrawn()
            : base()
        { }

        public FillingWithdrawn(Guid fillingId, string editorUserId)
            : base(fillingId, editorUserId)
        { }
    }
}
