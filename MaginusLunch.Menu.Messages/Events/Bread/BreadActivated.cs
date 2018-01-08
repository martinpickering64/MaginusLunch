using System;

namespace MaginusLunch.Menu.Messages.Events
{
    public class BreadActivated : MenuEvent
    {
        public BreadActivated() 
            : base()
        { }

        public BreadActivated(Guid breadId, string editorUserId) 
            : base(breadId, editorUserId)
        { }
    }
}
