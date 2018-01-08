using System;

namespace MaginusLunch.Menu.Messages.Events
{
    public class CalendarActivatedForDate : MenuEvent
    {
        public CalendarActivatedForDate(): base()
        {}

        public CalendarActivatedForDate(Guid calendarId, DateTime affectedDate, string editorUserId) : base(calendarId, editorUserId)
        {
            AffectedDate = affectedDate;
        }

        public DateTime AffectedDate { get; set; }
    }
}
