using System;

namespace MaginusLunch.Menu.Messages.Commands
{
    public class ActivateCalendarForDate : MenuCommand
    {
        public DateTime AffectedDate { get; set; }
    }
}
