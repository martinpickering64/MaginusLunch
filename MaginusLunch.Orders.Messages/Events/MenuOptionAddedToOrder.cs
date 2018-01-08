using System;

namespace MaginusLunch.Orders.Messages.Events
{
    public class MenuOptionAddedToOrder : OrderEvent
    {
        public Guid MenuOptionId { get; set; }
    }
}
