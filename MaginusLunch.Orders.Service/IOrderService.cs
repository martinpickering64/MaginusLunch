using MaginusLunch.Core;
using MaginusLunch.Core.Messages.Commands;
using MaginusLunch.Orders.Domain;
using MaginusLunch.Orders.Messages.Commands;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace MaginusLunch.Orders.Service
{
    public interface IOrderService
    {
        Task<CommandStatus> HandleForUserAsync(ClaimsPrincipal user, AnOrderCommand command, CancellationToken cancellationToken = default(CancellationToken));
        Task<Order> GetOrderAsync(Guid id, CancellationToken cancellationToken = default(CancellationToken));
        Task<Order> GetOrderForRecipientAsync(string recipientUserId, DateTime forDeliveryDate, CancellationToken cancellationToken = default(CancellationToken));
        Task<IEnumerable<Order>> GetOrdersAsync(DateTime forDeliveryDate, CancellationToken cancellationToken = default(CancellationToken));

    }
}
