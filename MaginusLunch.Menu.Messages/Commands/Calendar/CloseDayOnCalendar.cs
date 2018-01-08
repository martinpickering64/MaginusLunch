using System;

namespace MaginusLunch.Menu.Messages.Commands
{
    public class CloseDayOnCalendar : MenuCommand
    {
        public DayOfWeek AffectedDay { get; set; }
    }
}
