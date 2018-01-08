using System;

namespace MaginusLunch.Orders.Messages.Commands
{
    public class AddFillingToOrder : AnOrderCommand
    {
        public Guid FillingId { get; set; }
    }
}
