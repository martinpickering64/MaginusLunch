using System;

namespace MaginusLunch.Menu.Messages.Events
{
    public class DayOpenedOnCalendar : MenuEvent
    {
        public DayOpenedOnCalendar() : base()
        { }

        public DayOpenedOnCalendar(Guid calendarId, DayOfWeek dayNowOpen, string editorUserId)
            : base(calendarId, editorUserId)
        {
            DayNowOpen = dayNowOpen;
        }
        public DayOfWeek DayNowOpen { get; set; }
    }
}
