using System;

namespace MaginusLunch.Menu.Messages.Commands
{
    public class ActivateBreadForDate : MenuCommand
    {
        public DateTime AffectedDate { get; set; }
    }
}
