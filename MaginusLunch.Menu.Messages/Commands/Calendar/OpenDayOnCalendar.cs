using System;

namespace MaginusLunch.Menu.Messages.Commands
{
    public class OpenDayOnCalendar : MenuCommand
    {
        public DayOfWeek AffectedDay { get; set; }
    }
}
