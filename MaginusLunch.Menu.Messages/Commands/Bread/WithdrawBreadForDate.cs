using System;

namespace MaginusLunch.Menu.Messages.Commands
{
    public class WithdrawBreadForDate : MenuCommand
    {
        public DateTime AffectedDate { get; set; }
    }
}
