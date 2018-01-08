using System;
using MaginusLunch.Core;
using MaginusLunch.Core.Aggregates;
using MaginusLunch.Orders.Messages.Events;
using MaginusLunch.Orders.Messages.Commands;

namespace MaginusLunch.Orders.Domain
{
    public class OrderAggregate : AggregateRoot
    {
        public OrderAggregate(Guid id)
            : base()
        {
            Id = id;
            Status = OrderStatus.Incomplete;
        }

        #region Aggregate Properties

        public string OrderingUserId { get; private set; }
        public string RecipientUserId { get; private set; }
        public DateTime DeliveryDate { get; private set; }
        public Guid LunchProviderId { get; private set; }
        public Guid FillingId { get; private set; }
        public Guid BreadId { get; private set; }
        public Guid MenuOptionId { get; private set; }
        public OrderStatus Status { get; private set; }

        #endregion Aggregate Properties

        #region Command Behaviours

        public OrderAggregate(AddOrder addOrderCommand)
            : this(addOrderCommand.Id)
        {
            RaiseEvent(new OrderAdded
            {
                Id = addOrderCommand.Id,
                RecipientUserId = addOrderCommand.RecipientUserId,
                OrderingUserId = addOrderCommand.OrderingUserId,
                DeliveryDate = addOrderCommand.DeliveryDate,
                Status = Status
            });
        }

        public void CancelOrder()
        {
            RaiseEvent(new OrderCanceled
            {
                Id = Id
            });
        }

        public void MarkOrderAsCompleted()
        {
            RaiseEvent(new OrderCompleted
            {
                Id = Id
            });
        }

        public void MarkOrderAsIncomplete()
        {
            RaiseEvent(new OrderNowIncomplete
            {
                Id = Id
            });
        }

        public void RemoveBreadFromOrder()
        {
            RaiseEvent(new BreadRemovedFromOrder
            {
                Id = Id
            });
        }

        public void RemoveFillingFromOrder()
        {
            RaiseEvent(new FillingRemovedFromOrder
            {
                Id = Id
            });
        }

        public void RemoveMenuOptionFromOrder()
        {
            RaiseEvent(new MenuOptionRemovedFromOrder
            {
                Id = Id
            });
        }

        public void AddBreadToOrder(AddBreadToOrder command)
        {
            RaiseEvent(new BreadAddedToOrder
            {
                Id = Id,
                BreadId = command.BreadId
            });
        }

        public void AddFillingToOrder(AddFillingToOrder command)
        {
            RaiseEvent(new FillingAddedToOrder
            {
                Id = Id,
                FillingId = command.FillingId
            });
        }

        public void AddMenuOptionToOrder(AddMenuOptionToOrder command)
        {
            RaiseEvent(new MenuOptionAddedToOrder
            {
                Id = Id,
                MenuOptionId = command.MenuOptionId
            });
        }

        #endregion Command Behaviours

        #region Event Handlers

        public void Apply(OrderAdded theEvent)
        {
            Id = theEvent.Id;
            RecipientUserId = theEvent.RecipientUserId;
            DeliveryDate = theEvent.DeliveryDate;
        }

        public void Apply(BreadAddedToOrder theEvent)
        {
            BreadId = theEvent.BreadId;
        }

        public void Apply(BreadRemovedFromOrder theEvent)
        {
            BreadId = Guid.Empty;
        }

        public void Apply(FillingAddedToOrder theEvent)
        {
            FillingId = theEvent.FillingId;
        }

        public void Apply(FillingRemovedFromOrder theEvent)
        {
            FillingId = Guid.Empty;
        }

        public void Apply(MenuOptionAddedToOrder theEvent)
        {
            MenuOptionId = theEvent.MenuOptionId;
        }

        public void Apply(MenuOptionRemovedFromOrder theEvent)
        {
            MenuOptionId = Guid.Empty;
        }

        public void Apply(OrderCanceled theEvent)
        {
            Status = OrderStatus.Cancelled;
        }

        public void Apply(OrderCompleted theEvent)
        {
            Status = OrderStatus.Completed;
        }

        public void Apply(OrderNowIncomplete theEvent)
        {
            Status = OrderStatus.Incomplete;
        }

        #endregion Event Handlers
    }
}
