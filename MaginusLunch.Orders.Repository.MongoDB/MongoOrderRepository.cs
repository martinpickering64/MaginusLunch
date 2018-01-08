using MaginusLunch.MongoDB;
using MaginusLunch.Orders.Domain;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MaginusLunch.Orders.Repository
{
    public class MongoOrderRepository : MongoRepository<Order>, IOrderRepository
    {
        public MongoOrderRepository(MongoRepositorySettings settings,
                                    RetryPolicy asyncRetryPolicy,
                                    RetryPolicy retryPolicy)
            : base(settings, asyncRetryPolicy, retryPolicy)
        {
        }

        Order IOrderRepository.GetOrder(Guid id) => Get(id);

        public Task<Order> GetOrderAsync(Guid id, CancellationToken cancellationToken = default(CancellationToken)) 
            => GetAsync(id, cancellationToken);

        void IOrderRepository.SaveOrder(Order order)
        {
            order.Version++;
            if (order.Version == 1)
            {
                Insert(order);
            }
            else
            {
                Replace(order);
            }
        }

        public Task SaveOrderAsync(Order order, CancellationToken cancellationToken = default(CancellationToken))
        {
            order.Version++;
            if (order.Version == 1)
            {
                return InsertAsync(order, cancellationToken);
            }
            else
            {
                return ReplaceAsync(order, cancellationToken);
            }
        }

        public Order GetOrderForRecipient(string recipientUserId, DateTime forDeliveryDate)
        {
            var forDeliveryDateLower = forDeliveryDate.ToUniversalTime().Date;
            var forDeliveryDateUpper = forDeliveryDateLower.AddDays(1);
            return Find(o => o.RecipientUserId == recipientUserId
                             && o.DeliveryDate >= forDeliveryDateLower 
                             && o.DeliveryDate < forDeliveryDateUpper)
                       .FirstOrDefault();
        }

        public async Task<Order> GetOrderForRecipientAsync(string recipientUserId, DateTime forDeliveryDate, CancellationToken cancellationToken = default(CancellationToken))
        {
            var forDeliveryDateLower = forDeliveryDate.ToUniversalTime().Date;
            var forDeliveryDateUpper = forDeliveryDateLower.AddDays(1);
            var find = await FindAsync(o => o.RecipientUserId == recipientUserId
                                            && o.DeliveryDate >= forDeliveryDateLower && o.DeliveryDate < forDeliveryDateUpper, 
                                            cancellationToken).ConfigureAwait(false);
            return find.FirstOrDefault();
        }

        public IEnumerable<Order> GetOrders(DateTime forDeliveryDate)
        {
            var forDeliveryDateLower = forDeliveryDate.ToUniversalTime().Date;
            var forDeliveryDateUpper = forDeliveryDateLower.AddDays(1);
            return Find(o => o.DeliveryDate >= forDeliveryDateLower && o.DeliveryDate < forDeliveryDateUpper);
        }

        public Task<IEnumerable<Order>> GetOrdersAsync(DateTime forDeliveryDate, CancellationToken cancellationToken = default(CancellationToken))
        {
            var forDeliveryDateLower = forDeliveryDate.ToUniversalTime().Date;
            var forDeliveryDateUpper = forDeliveryDateLower.AddDays(1);
            return FindAsync(o => o.DeliveryDate >= forDeliveryDateLower && o.DeliveryDate < forDeliveryDateUpper, cancellationToken);
        }
    }
}
