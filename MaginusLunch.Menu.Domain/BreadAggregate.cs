using MaginusLunch.Core.Aggregates;
using MaginusLunch.Core.Entities;
using MaginusLunch.Menu.Messages.Commands;
using MaginusLunch.Menu.Messages.Events;
using System;
using System.Collections.Generic;

namespace MaginusLunch.Menu.Domain
{
    public class BreadAggregate : AggregateRoot
    {
        public BreadAggregate(Guid id)
            : base()
        {
            Id = id;
            WithdrawnDates = new DateTimeHashSet();
            Status = BreadStatus.Available;
        }

        #region Aggregate Properties

        public string EditorUserId { get; private set; }
        public Guid LunchProviderId { get; private set; }
        public string Name { get; private set; }
        public ISet<DateTime> WithdrawnDates { get; }
        public BreadStatus Status { get; private set; }

        #endregion Aggregate Properties

        #region Command Behaviours

        public BreadAggregate(CreateBread createBreadCommand)
            : this(createBreadCommand.Id)
        {
            RaiseEvent(new BreadCreated(createBreadCommand.Id, 
                                        createBreadCommand.Name, 
                                        createBreadCommand.LunchProviderId, 
                                        createBreadCommand.EditorUserId));
        }

        public void Activate(ActivateBread command)
        {
            RaiseEvent(new BreadActivated(command.Id, command.EditorUserId));
        }

        public void ActivateForDate(ActivateBreadForDate command)
        {
            RaiseEvent(new BreadActivatedForDate(command.Id, command.AffectedDate, command.EditorUserId));
        }

        public void Withdraw(WithdrawBread command)
        {
            RaiseEvent(new BreadWithdrawn(command.Id, command.EditorUserId));
        }

        public void WithdrawForDate(WithdrawBreadForDate command)
        {
            RaiseEvent(new BreadDateWithdrawn(command.Id, command.AffectedDate, command.EditorUserId));
        }

        public void ChangeName(ChangeNameOfBread command)
        {
            RaiseEvent(new BreadNameChanged(command.Id, command.NewName, command.EditorUserId));
        }

        #endregion Command Behaviours

        #region Event Handlers

        public void Apply(BreadCreated theEvent)
        {
            Id = theEvent.Id;
            EditorUserId = theEvent.EditorUserId;
            Name = theEvent.Name;
            LunchProviderId = theEvent.LunchProviderId;
            Status = theEvent.Status;
        }

        public void Apply(BreadActivated theEvent)
        {
            Status = BreadStatus.Available;
        }

        public void Apply(BreadActivatedForDate theEvent)
        {
            var theDate = theEvent.AffectedDate.Date;
            if (WithdrawnDates.Contains(theDate))
            {
                return;
            }
            WithdrawnDates.Add(theDate);
        }

        public void Apply(BreadWithdrawn theEvent)
        {
            Status = BreadStatus.Withdrawn;
        }
        public void Apply(BreadDateWithdrawn theEvent)
        {
            var affectedDate = theEvent.AffectedDate.Date;
            if (!WithdrawnDates.Contains(affectedDate))
            {
                return;
            }
            WithdrawnDates.Remove(affectedDate);
        }

        public void Apply(BreadNameChanged theEvent)
        {
            Name = theEvent.Name;
        }

        #endregion Event Handlers

    }
}
