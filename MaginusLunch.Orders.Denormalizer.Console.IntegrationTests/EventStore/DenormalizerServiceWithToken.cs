using System;
using System.Threading;
using EventStore.ClientAPI;

namespace MaginusLunch.Orders.Denormalizer.Console.IntegrationTests.EventStore
{
    public class DenormalizerServiceWithToken : DenormalizerService
    {
        public DenormalizerServiceWithToken(OrderEventHandlers handlers, 
            IEventStoreConnection eventStoreConnection, 
            DenormalizerServiceSettings settings) 
            : base(handlers, eventStoreConnection, settings)
        {
        }

        public DenormalizerServiceWithToken(OrderEventHandlers handlers, 
            IEventStoreConnection eventStoreConnection, 
            DenormalizerServiceSettings settings,
            Action<EventStorePersistentSubscriptionBase, SubscriptionDropReason, Exception> subscriptionDroppedHandler) 
            : base(handlers, 
                  eventStoreConnection, 
                  settings, 
                  subscriptionDroppedHandler)
        {
        }

        public CancellationTokenSource TokenSrc => base.StopServiceTokenSrc;
    }
}
