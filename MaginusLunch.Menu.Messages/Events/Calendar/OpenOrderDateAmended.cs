using System;

namespace MaginusLunch.Menu.Messages.Events
{
    public class OpenOrderDateAmended : MenuEvent
    {
        public OpenOrderDateAmended(): base()
        {}

        public OpenOrderDateAmended(Guid calendarId, DateTime newOpenOrderDate, string editorUserId) : base(calendarId, editorUserId)
        {
            NewOpenOrderDate = newOpenOrderDate;
        }

        public DateTime NewOpenOrderDate { get; set; }
    }
}
