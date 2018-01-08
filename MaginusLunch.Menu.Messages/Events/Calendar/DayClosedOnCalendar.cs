using System;

namespace MaginusLunch.Menu.Messages.Events
{
    public class DayClosedOnCalendar : MenuEvent
    {
        public DayClosedOnCalendar() : base()
        { }

        public DayClosedOnCalendar(Guid calendarId, DayOfWeek dayNowClosed, string editorUserId)
            : base(calendarId, editorUserId)
        {
            DayNowClosed = dayNowClosed;
        }
        public DayOfWeek DayNowClosed { get; set; }
    }
}
