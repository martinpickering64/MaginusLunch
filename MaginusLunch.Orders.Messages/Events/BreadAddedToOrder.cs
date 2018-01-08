using System;

namespace MaginusLunch.Orders.Messages.Events
{
    public class BreadAddedToOrder : OrderEvent
    {
        public Guid BreadId { get; set; }
    }
}
