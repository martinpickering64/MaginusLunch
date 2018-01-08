using System;

namespace MaginusLunch.Menu.Messages.Events
{
    public class CalendarCreated : MenuEvent
    {
        public CalendarCreated() : base()
        { }

        public CalendarCreated(Guid id, string editorUserId) : base(id, editorUserId)
        { }
    }
}
