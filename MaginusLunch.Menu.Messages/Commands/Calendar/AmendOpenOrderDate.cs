using System;

namespace MaginusLunch.Menu.Messages.Commands
{
    public class AmendOpenOrderDate : MenuCommand
    {
        public DateTime NewOpenOrderDate { get; set; }
    }
}
