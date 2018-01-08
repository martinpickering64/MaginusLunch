using System;

namespace MaginusLunch.Orders.API.Authorization
{
    public class OrderCommandAuthorizationResource
    {
        public OrderCommandAuthorizationResource(Domain.Order order, Messages.Commands.AnOrderCommand command)
        {
            //Order = order ?? throw new ArgumentNullException("order");
            Command = command ?? throw new ArgumentNullException("command");
        }

        public Domain.Order Order { get; }
        public Messages.Commands.AnOrderCommand Command { get;}
    }
}
