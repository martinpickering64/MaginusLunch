using MaginusLunch.Core;
using MaginusLunch.Core.EventStore;
using MaginusLunch.Core.PublishSubscribe;
using MaginusLunch.Orders.Domain;
using MaginusLunch.Orders.Messages.Commands;
using MaginusLunch.Orders.Messages.Events;
using MaginusLunch.Orders.Repository;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MaginusLunch.Orders.Denormalizer
{
    public class OrderEventHandlers : IHandleEvent<OrderAdded>,
                                        IHandleEvent<OrderCanceled>,
                                        IHandleEvent<OrderCompleted>,
                                        IHandleEvent<OrderNowIncomplete>,
                                        IHandleEvent<BreadAddedToOrder>,
                                        IHandleEvent<BreadRemovedFromOrder>,
                                        IHandleEvent<FillingAddedToOrder>,
                                        IHandleEvent<FillingRemovedFromOrder>,
                                        IHandleEvent<MenuOptionAddedToOrder>,
                                        IHandleEvent<MenuOptionRemovedFromOrder>
    {
        private readonly IOrderRepository _repository;
        private readonly ILunchProviderRepository _lunchProviders;
        private readonly IFillingRepository _fillings;
        private readonly IBreadRepository _breads;
        private readonly IMenuOptionRepository _menuOptions;
        private readonly ICalendarRepository _calendarRepository;
        private readonly IRepository _eventStoreRepository;

        public OrderEventHandlers(IOrderRepository repository,
            ILunchProviderRepository lunchProviders,
            IFillingRepository fillings,
            IBreadRepository breads,
            IMenuOptionRepository menuOptions,
            ICalendarRepository calendarRepository,
            IRepository eventStoreRepository)
        {
            _repository = repository;
            _lunchProviders = lunchProviders;
            _fillings = fillings;
            _breads = breads;
            _menuOptions = menuOptions;
            _calendarRepository = calendarRepository;
            _eventStoreRepository = eventStoreRepository;
        }

        public virtual async Task HandleAsync(OrderAdded theEvent, CancellationToken cancellationToken = default(CancellationToken))
        {
            var order = new Order(theEvent.Id)
            {
                OrderingUserId = theEvent.OrderingUserId,
                RecipientUserId = theEvent.RecipientUserId,
                DeliveryDate = theEvent.DeliveryDate,
                Status = theEvent.Status
            };
            var existingOrder = await _repository.GetOrderAsync(theEvent.Id, cancellationToken)
                                            .ConfigureAwait(false);
            if (existingOrder != null)
            {
                order.Version = existingOrder.Version;
                order.Status = existingOrder.Status;
            }
            var task = _repository.SaveOrderAsync(order, cancellationToken);
            var dateWithdrawn = await RequiredDateIsStillAvailableAsync(theEvent.DeliveryDate).ConfigureAwait(false);
            if (dateWithdrawn)
            {
                var orderAggregate = await _eventStoreRepository.GetByIdAsync<OrderAggregate>(order.Id).ConfigureAwait(false);
                orderAggregate.CancelOrder();
            }
            await task.ConfigureAwait(false);
        }

        public void Handle(OrderAdded theEvent)
        {
            HandleAsync(theEvent).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public void Handle(OrderCanceled theEvent)
        {
            HandleAsync(theEvent).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public async Task HandleAsync(OrderCanceled theEvent, CancellationToken cancellationToken = default(CancellationToken))
        {
            var order = await _repository.GetOrderAsync(theEvent.Id, cancellationToken).ConfigureAwait(false);
            if (order == null)
            {
                order = new Order(theEvent.Id) { Version = 1 };
            }
            order.OrderingUserId = theEvent.OrderingUserId;
            order.Status = OrderStatus.Cancelled;
            await _repository.SaveOrderAsync(order, cancellationToken).ConfigureAwait(false);
        }

        public void Handle(OrderCompleted theEvent)
        {
            HandleAsync(theEvent).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public async Task HandleAsync(OrderCompleted theEvent, CancellationToken cancellationToken = default(CancellationToken))
        {
            var order = await _repository.GetOrderAsync(theEvent.Id, cancellationToken).ConfigureAwait(false);
            if (order == null)
            {
                order = new Order(theEvent.Id) { Version = 1 };
            }
            order.OrderingUserId = theEvent.OrderingUserId;
            order.Status = OrderStatus.Completed;
            await _repository.SaveOrderAsync(order, cancellationToken).ConfigureAwait(false);
        }

        public void Handle(OrderNowIncomplete theEvent)
        {
            HandleAsync(theEvent).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public async Task HandleAsync(OrderNowIncomplete theEvent, CancellationToken cancellationToken = default(CancellationToken))
        {
            var order = await _repository.GetOrderAsync(theEvent.Id, cancellationToken).ConfigureAwait(false);
            if (order == null)
            {
                order = new Order(theEvent.Id) { Version = 1 };
            }
            order.OrderingUserId = theEvent.OrderingUserId;
            order.Status = OrderStatus.Incomplete;
            await _repository.SaveOrderAsync(order, cancellationToken).ConfigureAwait(false);
        }

        public void Handle(BreadAddedToOrder theEvent)
        {
            HandleAsync(theEvent).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public async Task HandleAsync(BreadAddedToOrder theEvent, CancellationToken cancellationToken = default(CancellationToken))
        {
            var order = await _repository.GetOrderAsync(theEvent.Id, cancellationToken).ConfigureAwait(false);
            if (order == null)
            {
                order = new Order(theEvent.Id) { Version = 1, Status = OrderStatus.Incomplete };
            }
            var bread = await _breads.GetBreadAsync(theEvent.BreadId, cancellationToken).ConfigureAwait(false);
            order.BreadId = bread.Id;
            order.Bread = bread.Name;
            order.OrderingUserId = theEvent.OrderingUserId;
            await _repository.SaveOrderAsync(order, cancellationToken).ConfigureAwait(false);
        }

        public void Handle(BreadRemovedFromOrder theEvent)
        {
            HandleAsync(theEvent).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public async Task HandleAsync(BreadRemovedFromOrder theEvent, CancellationToken cancellationToken = default(CancellationToken))
        {
            var order = await _repository.GetOrderAsync(theEvent.Id, cancellationToken).ConfigureAwait(false);
            if (order == null)
            {
                order = new Order(theEvent.Id) { Version = 1, Status = OrderStatus.Incomplete };
            }
            order.BreadId = Guid.Empty;
            order.Bread = null;
            order.OrderingUserId = theEvent.OrderingUserId;
            await _repository.SaveOrderAsync(order, cancellationToken).ConfigureAwait(false);
        }

        public void Handle(FillingAddedToOrder theEvent)
        {
            HandleAsync(theEvent).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public async Task HandleAsync(FillingAddedToOrder theEvent, CancellationToken cancellationToken = default(CancellationToken))
        {
            var order = await _repository.GetOrderAsync(theEvent.Id, cancellationToken).ConfigureAwait(false);
            if (order == null)
            {
                order = new Order(theEvent.Id) { Version = 1, Status = OrderStatus.Incomplete };
            }
            var filling = await _fillings.GetFillingAsync(theEvent.FillingId, cancellationToken).ConfigureAwait(false);
            order.FillingId = filling.Id;
            order.Filling = filling.Name;
            order.OrderingUserId = theEvent.OrderingUserId;
            var task = _repository.SaveOrderAsync(order, cancellationToken);
            if ((filling.DisallowBread && order.BreadId != Guid.Empty)
                || (filling.DisallowMenuOption && order.MenuOptionId != Guid.Empty))
            {
                var orderAggregate = await _eventStoreRepository.GetByIdAsync<OrderAggregate>(order.Id).ConfigureAwait(false);
                if (filling.DisallowBread && order.BreadId != Guid.Empty)
                {
                    orderAggregate.RemoveBreadFromOrder();
                }
                if (filling.DisallowMenuOption && order.MenuOptionId != Guid.Empty)
                {
                    orderAggregate.RemoveMenuOptionFromOrder();
                }
            }
            await task.ConfigureAwait(false);
        }

        public void Handle(FillingRemovedFromOrder theEvent)
        {
            HandleAsync(theEvent).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public async Task HandleAsync(FillingRemovedFromOrder theEvent, CancellationToken cancellationToken = default(CancellationToken))
        {
            var order = await _repository.GetOrderAsync(theEvent.Id, cancellationToken).ConfigureAwait(false);
            if (order == null)
            {
                order = new Order(theEvent.Id) { Version = 1, Status = OrderStatus.Incomplete };
            }
            order.FillingId = Guid.Empty;
            order.Filling = null;
            order.OrderingUserId = theEvent.OrderingUserId;
            await _repository.SaveOrderAsync(order, cancellationToken).ConfigureAwait(false);
        }

        public void Handle(MenuOptionAddedToOrder theEvent)
        {
            HandleAsync(theEvent).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public async Task HandleAsync(MenuOptionAddedToOrder theEvent, CancellationToken cancellationToken = default(CancellationToken))
        {
            var order = await _repository.GetOrderAsync(theEvent.Id, cancellationToken).ConfigureAwait(false);
            if (order == null)
            {
                order = new Order(theEvent.Id) { Version = 1, Status = OrderStatus.Incomplete };
            }
            var menuOption = await _menuOptions.GetMenuOptionAsync(theEvent.MenuOptionId, cancellationToken).ConfigureAwait(false);
            order.MenuOptionId = menuOption.Id;
            order.MenuOption = menuOption.Name;
            order.OrderingUserId = theEvent.OrderingUserId;
            await _repository.SaveOrderAsync(order, cancellationToken).ConfigureAwait(false);
        }

        public void Handle(MenuOptionRemovedFromOrder theEvent)
        {
            HandleAsync(theEvent).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public async Task HandleAsync(MenuOptionRemovedFromOrder theEvent, CancellationToken cancellationToken = default(CancellationToken))
        {
            var order = await _repository.GetOrderAsync(theEvent.Id, cancellationToken).ConfigureAwait(false);
            if (order == null)
            {
                order = new Order(theEvent.Id) { Version = 1, Status = OrderStatus.Incomplete };
            }
            order.MenuOptionId = Guid.Empty;
            order.MenuOption = null;
            order.OrderingUserId = theEvent.OrderingUserId;
            await _repository.SaveOrderAsync(order, cancellationToken).ConfigureAwait(false);
        }

        private async Task<bool> RequiredDateIsStillAvailableAsync(DateTime requiredDate)
        {
            requiredDate = requiredDate.ToUniversalTime().Date;
            var calendar = await _calendarRepository.GetCalendarAsync().ConfigureAwait(false);
            if (calendar == null
                || calendar.OrdersStillOpenBeyond > requiredDate
                || !calendar.AvailableDays[(int)requiredDate.DayOfWeek]
                || calendar.WithdrawnDates.Any(wd => wd == requiredDate))
            {
                return false;
            }
            return true;
        }
    }
}
