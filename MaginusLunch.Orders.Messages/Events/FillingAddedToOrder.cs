using System;

namespace MaginusLunch.Orders.Messages.Events
{
    public class FillingAddedToOrder : OrderEvent
    {
        public Guid FillingId { get; set; }
    }
}
