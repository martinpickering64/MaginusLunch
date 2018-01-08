using System;

namespace MaginusLunch.Menu.Messages.Events
{
    public class CalendarDateWithdrawn : MenuEvent
    {
        public CalendarDateWithdrawn(): base()
        {}

        public CalendarDateWithdrawn(Guid calendarId, DateTime affectedDate, string editorUserId) : base(calendarId, editorUserId)
        {
            AffectedDate = affectedDate;
        }

        public DateTime AffectedDate { get; set; }
    }
}
