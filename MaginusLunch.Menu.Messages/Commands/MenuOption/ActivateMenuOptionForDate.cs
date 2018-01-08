using System;

namespace MaginusLunch.Menu.Messages.Commands
{
    public class ActivateMenuOptionForDate : MenuCommand
    {
        public DateTime AffectedDate { get; set; }
    }
}
