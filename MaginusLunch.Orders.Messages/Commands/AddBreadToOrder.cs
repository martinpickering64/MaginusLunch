using System;

namespace MaginusLunch.Orders.Messages.Commands
{
    public class AddBreadToOrder : AnOrderCommand
    {
        public Guid BreadId { get; set; }
    }
}
