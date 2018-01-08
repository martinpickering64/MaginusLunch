using System;

namespace MaginusLunch.Menu.Messages.Commands
{
    public class WithdrawMenuOptionForDate : MenuCommand
    {
        public DateTime AffectedDate { get; set; }
    }
}
