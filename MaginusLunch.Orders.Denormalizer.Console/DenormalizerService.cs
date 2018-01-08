using System.Threading;
using MaginusLunch.Core.ServiceManagement;
using MaginusLunch.Logging;
using System.Threading.Tasks;
using System;
using EventStore.ClientAPI;
using MaginusLunch.Orders.Messages.Events;
using MaginusLunch.Core.Aggregates;

namespace MaginusLunch.Orders.Denormalizer.Console
{
    /// <remarks>
    /// This service depends on a GetEventStore Projection called MaginusLunch-Orders...
    /// 
    /// fromCategory("orderAggregate").foreachStream().when({
    /// $any: function(s, ev)
    /// {
    ///    linkTo("MaginusLunch-Orders", ev);
    /// }
    /// });
    /// 
    /// It also depends upon a Persistent Subscription...
    /// 
    /// GroupName: Denormalizer
    /// StreamName: MaginusLunch-Orders
    /// </remarks>
    public class DenormalizerService : DaemonServiceBase
    {
        private static ILog _logger = LogManager.GetLogger(typeof(DenormalizerService));

        private readonly OrderEventHandlers _handlers;
        private readonly IEventStoreConnection _eventStoreConnection;
        private EventStorePersistentSubscriptionBase _subscription;
        private readonly string _streamName;
        private readonly string _groupName;
        private readonly TimeSpan _timeAllowedToShutdown;
        private Action<EventStorePersistentSubscriptionBase, SubscriptionDropReason, Exception> SubscriptionDroppedHandler { get; set; }


        public DenormalizerService(OrderEventHandlers handlers, IEventStoreConnection eventStoreConnection)
            : this(handlers, eventStoreConnection, new DenormalizerServiceSettings())
        { }

        public DenormalizerService(OrderEventHandlers handlers,
            IEventStoreConnection eventStoreConnection,
            DenormalizerServiceSettings settings)
            : this(handlers, eventStoreConnection, settings, null)
        { }

        public DenormalizerService(OrderEventHandlers handlers, 
            IEventStoreConnection eventStoreConnection, 
            DenormalizerServiceSettings settings,
            Action<EventStorePersistentSubscriptionBase, SubscriptionDropReason, Exception> subscriptionDroppedHandler)
            :base(_logger)
        {
            _handlers = handlers;
            _eventStoreConnection = eventStoreConnection;
            _streamName = settings.StreamName;
            _groupName = settings.GroupName;
            _timeAllowedToShutdown = settings.TimeAllowedToShutdown;
            if (subscriptionDroppedHandler == null)
            {
                SubscriptionDroppedHandler = DefaultSubscriptionDropped;
            }
            else
            {
                SubscriptionDroppedHandler = subscriptionDroppedHandler;
            }
        }

        protected override bool Start(CancellationTokenSource useThisCancellationTokenSrc)
        {
            try
            {
                _subscription = ListenForOrdersEvents(useThisCancellationTokenSrc.Token);
            }
            catch (AggregateException aggEx)
            {
                LogAggregateException(aggEx);
                throw;
            }
            return true;
        }

        protected override bool Stop(CancellationTokenSource useThisCancellationTokenSrc)
        {
            useThisCancellationTokenSrc.Cancel();
            if (_subscription != null)
            {
                try
                {
                    _subscription.Stop(_timeAllowedToShutdown);
                    _subscription = null;
                }
                catch (Exception ex)
                {
                    _logger.Error("Problem whilst attempting to stop the GetEventStore Subscription.",
                        ex);
                    throw;
                }
            }
            return true;
        }

        private EventStorePersistentSubscriptionBase ListenForOrdersEvents(CancellationToken cancellationToken)
        {

            return _eventStoreConnection.ConnectToPersistentSubscription(_streamName,
                                        _groupName,
                                        (sub, e) => EventAppeared(sub, e, cancellationToken),
                                        (sub, reason, ex) => SubscriptionDroppedHandler(sub, reason, ex),
                                        bufferSize: 10,
                                        autoAck: false);
        }

        private async Task EventAppeared(EventStorePersistentSubscriptionBase subscription, 
            ResolvedEvent anEvent,
            CancellationToken cancellationToken)
        {
            var deserialisedEvent = anEvent.DeserializeEvent();
            var typeOfEvent = deserialisedEvent.GetType();
            try
            {

                if (typeOfEvent == typeof(OrderAdded))
                {
                    await _handlers.HandleAsync(deserialisedEvent as OrderAdded, cancellationToken)
                        .ConfigureAwait(false);
                }
                else if (typeOfEvent == typeof(OrderCanceled))
                {
                    await _handlers.HandleAsync(deserialisedEvent as OrderCanceled, cancellationToken)
                        .ConfigureAwait(false);
                }
                else if (typeOfEvent == typeof(OrderCompleted))
                {
                    await _handlers.HandleAsync(deserialisedEvent as OrderCompleted, cancellationToken)
                        .ConfigureAwait(false);
                }
                else if (typeOfEvent == typeof(OrderNowIncomplete))
                {
                    await _handlers.HandleAsync(deserialisedEvent as OrderNowIncomplete, cancellationToken)
                        .ConfigureAwait(false);
                }
                else if (typeOfEvent == typeof(BreadAddedToOrder))
                {
                    await _handlers.HandleAsync(deserialisedEvent as BreadAddedToOrder, cancellationToken)
                        .ConfigureAwait(false);
                }
                else if (typeOfEvent == typeof(BreadRemovedFromOrder))
                {
                    await _handlers.HandleAsync(deserialisedEvent as BreadRemovedFromOrder, cancellationToken)
                        .ConfigureAwait(false);
                }
                else if (typeOfEvent == typeof(FillingAddedToOrder))
                {
                    await _handlers.HandleAsync(deserialisedEvent as FillingAddedToOrder, cancellationToken)
                        .ConfigureAwait(false);
                }
                else if (typeOfEvent == typeof(FillingRemovedFromOrder))
                {
                    await _handlers.HandleAsync(deserialisedEvent as FillingRemovedFromOrder, cancellationToken)
                        .ConfigureAwait(false);
                }
                else if (typeOfEvent == typeof(MenuOptionAddedToOrder))
                {
                    await _handlers.HandleAsync(deserialisedEvent as MenuOptionAddedToOrder, cancellationToken)
                        .ConfigureAwait(false);
                }
                else if (typeOfEvent == typeof(MenuOptionRemovedFromOrder))
                {
                    await _handlers.HandleAsync(deserialisedEvent as MenuOptionRemovedFromOrder, cancellationToken)
                        .ConfigureAwait(false);
                }
                else
                {
                    var msg = "The Event Type of {0} is unexpected/unknown to me. The Event Number was #{1} and Id {2}";
                    _logger.FatalFormat(msg, typeOfEvent.Name, anEvent.OriginalEventNumber, anEvent.OriginalEvent.EventId);
                    var msg1 = string.Format(msg, typeOfEvent.Name, anEvent.OriginalEventNumber, anEvent.OriginalEvent.EventId);
                    subscription.Fail(anEvent, PersistentSubscriptionNakEventAction.Stop, msg1);
                    throw new HandlerForDomainEventNotFoundException(msg1);
                }
            }
            catch (OperationCanceledException)
            { }
            catch(HandlerForDomainEventNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                const string msg = "An unexpected type of exception occurred whilst handling an Event Type of {0}. The Event Number was #{1} and Id {2}";
                _logger.FatalFormat(msg, typeOfEvent.Name, anEvent.OriginalEventNumber, anEvent.OriginalEvent.EventId, ex);
                try { subscription.Fail(anEvent, PersistentSubscriptionNakEventAction.Stop, msg); }
                catch (Exception inEx)
                {
                    _logger.FatalFormat("Failed to 'fail' the subscription for an Event of Type {0}. The Event Number was #{1} and Id {2}",
                        typeOfEvent.Name, anEvent.OriginalEventNumber, anEvent.OriginalEvent.EventId, inEx);
                }
                throw;
            }
            if (cancellationToken.IsCancellationRequested)
            {
                subscription.Fail(anEvent, 
                    PersistentSubscriptionNakEventAction.Stop, 
                    "The operation was canceled");
            }
            else
            {
                _logger.DebugFormat("Acknowledging Event #{0} - {1}.", anEvent.OriginalEvent.EventNumber, anEvent.Event.EventId);
                subscription.Acknowledge(anEvent);
            }
        }
            
        public static void DefaultSubscriptionDropped(EventStorePersistentSubscriptionBase subscription, 
            SubscriptionDropReason reason, Exception exception)
        {
            _logger.WarnFormat("Subscription has dropped! Reason stated was {0}.", reason, exception);
            if (reason != SubscriptionDropReason.UserInitiated)
            {
                throw new ApplicationException($"Subscription has dropped! Reason stated was {reason}.", exception);
            }
        }

        private void LogAggregateException(Exception theException, 
            string targetSite = null, 
            string source = null,
            string stackTrace = null)
        {
            if (theException is AggregateException)
            {
                foreach (var ex in ((AggregateException)theException).InnerExceptions)
                {
                    LogAggregateException(ex, theException.TargetSite.ToString(), theException.Source, theException.StackTrace);
                }
            }
            else
            {
                _logger.FatalFormat("Failed to obtain Persistent Subscription to stream [{0}] as group [{1}] due to target:{2} source:{3}[{4}]: {5}\r\nStack trace: {6}",
                        _streamName,
                        _groupName,
                        theException.TargetSite == null ? targetSite : theException.TargetSite.ToString(),
                        theException.Source ?? source,
                        theException.HResult,
                        theException.Message,
                        theException.StackTrace ?? stackTrace,
                        theException);
            }
        }
    }
}