using System;

namespace MaginusLunch.Menu.Messages.Commands
{
    public class ActivateFillingForDate : MenuCommand
    {
        public DateTime AffectedDate { get; set; }
    }
}
