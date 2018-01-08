using System;

namespace MaginusLunch.Orders.Messages.Events
{
    public abstract class OrderEvent
    {
        public Guid Id { get; set; }
        public string OrderingUserId { get; set; }
    }
}
