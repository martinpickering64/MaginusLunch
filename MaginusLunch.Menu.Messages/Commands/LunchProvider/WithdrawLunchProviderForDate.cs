using System;

namespace MaginusLunch.Menu.Messages.Commands
{
    public class WithdrawLunchProviderForDate : MenuCommand
    {
        public DateTime AffectedDate { get; set; }
    }
}
