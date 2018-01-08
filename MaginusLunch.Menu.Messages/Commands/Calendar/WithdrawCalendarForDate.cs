using System;

namespace MaginusLunch.Menu.Messages.Commands
{
    public class WithdrawCalendarForDate : MenuCommand
    {
        public DateTime AffectedDate { get; set; }
    }
}
