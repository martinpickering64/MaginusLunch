using MaginusLunch.Orders.Domain;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MaginusLunch.Orders.Repository
{
    public interface IOrderRepository
    {
        Order GetOrder(Guid id);
        Task<Order> GetOrderAsync(Guid id, CancellationToken cancellationToken = default(CancellationToken));
        Order GetOrderForRecipient(string recipientUserId, DateTime forDeliveryDate);
        Task<Order> GetOrderForRecipientAsync(string recipientUserId, DateTime forDeliveryDate, CancellationToken cancellationToken = default(CancellationToken));
        IEnumerable<Order> GetOrders(DateTime forDeliveryDate);
        Task<IEnumerable<Order>> GetOrdersAsync(DateTime forDeliveryDate, CancellationToken cancellationToken = default(CancellationToken));
        void SaveOrder(Order order);
        Task SaveOrderAsync(Order order, CancellationToken cancellationToken = default(CancellationToken));
    }
}
