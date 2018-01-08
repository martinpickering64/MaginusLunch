using System;

namespace MaginusLunch.Orders.Messages.Commands
{
    public class AddOrder : AnOrderCommand
    {
        public string RecipientUserId { get; set; }
        public DateTime DeliveryDate { get; set; }
    }
}
