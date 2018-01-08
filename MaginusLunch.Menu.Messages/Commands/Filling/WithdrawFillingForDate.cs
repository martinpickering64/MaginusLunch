using System;

namespace MaginusLunch.Menu.Messages.Commands
{
    public class WithdrawFillingForDate : MenuCommand
    {
        public DateTime AffectedDate { get; set; }
    }
}
