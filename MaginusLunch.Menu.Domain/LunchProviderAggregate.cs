using MaginusLunch.Core.Aggregates;
using MaginusLunch.Core.Entities;
using MaginusLunch.Menu.Messages.Commands;
using MaginusLunch.Menu.Messages.Events;
using System;
using System.Collections.Generic;

namespace MaginusLunch.Menu.Domain
{
    public class LunchProviderAggregate : AggregateRoot
    {
        public LunchProviderAggregate(Guid id)
            : base()
        {
            Id = id;
            WithdrawnDates = new DateTimeHashSet();
            Status = LunchProviderStatus.Available;
        }

        #region Aggregate Properties

        public string EditorUserId { get; private set; }
        public string Name { get; private set; }
        public ISet<DateTime> WithdrawnDates { get; }
        public LunchProviderStatus Status { get; private set; }

        #endregion Aggregate Properties

        #region Command Behaviours

        public LunchProviderAggregate(CreateLunchProvider command)
            : this(command.Id)
        {
            RaiseEvent(new LunchProviderCreated(command.Id, command.Name, command.EditorUserId));
        }

        public void Activate(ActivateLunchProvider command)
        {
            RaiseEvent(new LunchProviderActivated(command.Id, command.EditorUserId));
        }

        public void ActivateForDate(ActivateLunchProviderForDate command)
        {
            RaiseEvent(new LunchProviderActivatedForDate(command.Id, command.AffectedDate, command.EditorUserId));
        }

        public void Withdraw(WithdrawLunchProvider command)
        {
            RaiseEvent(new LunchProviderWithdrawn(command.Id, command.EditorUserId));
        }

        public void WithdrawForDate(WithdrawLunchProviderForDate command)
        {
            RaiseEvent(new LunchProviderDateWithdrawn(command.Id, command.AffectedDate, command.EditorUserId));
        }

        public void ChangeName(ChangeNameOfLunchProvider command)
        {
            RaiseEvent(new LunchProviderNameChanged(command.Id, command.NewName, command.EditorUserId));
        }

        #endregion Command Behaviours

        #region Event Handlers

        public void Apply(LunchProviderCreated theEvent)
        {
            Id = theEvent.Id;
            EditorUserId = theEvent.EditorUserId;
            Name = theEvent.Name;
            Status = theEvent.Status;
        }
        public void Apply(LunchProviderActivated theEvent)
        {
            Status = LunchProviderStatus.Available;
        }

        public void Apply(LunchProviderActivatedForDate theEvent)
        {
            var theDate = theEvent.AffectedDate.Date;
            if (WithdrawnDates.Contains(theDate))
            {
                return;
            }
            WithdrawnDates.Add(theDate);
        }

        public void Apply(LunchProviderWithdrawn theEvent)
        {
            Status = LunchProviderStatus.Withdrawn;
        }
        public void Apply(LunchProviderDateWithdrawn theEvent)
        {
            var affectedDate = theEvent.AffectedDate.Date;
            if (!WithdrawnDates.Contains(affectedDate))
            {
                return;
            }
            WithdrawnDates.Remove(affectedDate);
        }

        public void Apply(LunchProviderNameChanged theEvent)
        {
            Name = theEvent.Name;
        }

        #endregion Event Handlers
    }
}
