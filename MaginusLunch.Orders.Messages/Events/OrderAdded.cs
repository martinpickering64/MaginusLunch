using MaginusLunch.Core;
using System;

namespace MaginusLunch.Orders.Messages.Events
{
    public class OrderAdded : OrderEvent
    {
        public string RecipientUserId { get; set; }
        public DateTime DeliveryDate { get; set; }
        public OrderStatus Status { get; set; }
    }
}
