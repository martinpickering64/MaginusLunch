using System;

namespace MaginusLunch.Orders.Messages.Commands
{
    public class AddMenuOptionToOrder : AnOrderCommand
    {
        public Guid MenuOptionId { get; set; }
    }
}
