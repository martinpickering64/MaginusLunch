using System;

namespace MaginusLunch.Menu.Messages.Events
{
    public class BreadWithdrawn : MenuEvent
    {
        public BreadWithdrawn() 
            : base()
        {
        }

        public BreadWithdrawn(Guid breadId, string editorUserId) 
            : base(breadId, editorUserId)
        {
        }
    }
}
