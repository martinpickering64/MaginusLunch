using MaginusLunch.Core.Aggregates;
using MaginusLunch.Core.Entities;
using MaginusLunch.Menu.Messages.Commands;
using MaginusLunch.Menu.Messages.Events;
using System;
using System.Collections.Generic;

namespace MaginusLunch.Menu.Domain
{
    public class MenuOptionAggregate : AggregateRoot
    {
        public MenuOptionAggregate(Guid id)
            : base()
        {
            Id = id;
            WithdrawnDates = new DateTimeHashSet();
            Status = MenuOptionStatus.Available;
        }

        #region Aggregate Properties

        public string EditorUserId { get; private set; }
        public Guid LunchProviderId { get; private set; }
        public string Name { get; private set; }
        public ISet<DateTime> WithdrawnDates { get; }
        public MenuOptionStatus Status { get; private set; }

        #endregion Aggregate Properties

        #region Command Behaviours

        public MenuOptionAggregate(CreateMenuOption createMenuOptionCommand)
            : this(createMenuOptionCommand.Id)
        {
            RaiseEvent(new MenuOptionCreated(createMenuOptionCommand.Id, 
                                        createMenuOptionCommand.Name, 
                                        createMenuOptionCommand.LunchProviderId, 
                                        createMenuOptionCommand.EditorUserId));
        }

        public void Activate(ActivateMenuOption command)
        {
            RaiseEvent(new MenuOptionActivated(command.Id, command.EditorUserId));
        }

        public void ActivateForDate(ActivateMenuOptionForDate command)
        {
            RaiseEvent(new MenuOptionActivatedForDate(command.Id, command.AffectedDate, command.EditorUserId));
        }

        public void Withdraw(WithdrawMenuOption command)
        {
            RaiseEvent(new MenuOptionWithdrawn(command.Id, command.EditorUserId));
        }

        public void WithdrawForDate(WithdrawMenuOptionForDate command)
        {
            RaiseEvent(new MenuOptionDateWithdrawn(command.Id, command.AffectedDate, command.EditorUserId));
        }

        public void ChangeName(ChangeNameOfMenuOption command)
        {
            RaiseEvent(new MenuOptionNameChanged(command.Id, command.NewName, command.EditorUserId));
        }

        #endregion Command Behaviours

        #region Event Handlers

        public void Apply(MenuOptionCreated theEvent)
        {
            Id = theEvent.Id;
            EditorUserId = theEvent.EditorUserId;
            Name = theEvent.Name;
            LunchProviderId = theEvent.LunchProviderId;
            Status = theEvent.Status;
        }

        public void Apply(MenuOptionActivated theEvent)
        {
            Status = MenuOptionStatus.Available;
        }

        public void Apply(MenuOptionActivatedForDate theEvent)
        {
            var theDate = theEvent.AffectedDate.Date;
            if (WithdrawnDates.Contains(theDate))
            {
                return;
            }
            WithdrawnDates.Add(theDate);
        }

        public void Apply(MenuOptionWithdrawn theEvent)
        {
            Status = MenuOptionStatus.Withdrawn;
        }
        public void Apply(MenuOptionDateWithdrawn theEvent)
        {
            var affectedDate = theEvent.AffectedDate.Date;
            if (!WithdrawnDates.Contains(affectedDate))
            {
                return;
            }
            WithdrawnDates.Remove(affectedDate);
        }

        public void Apply(MenuOptionNameChanged theEvent)
        {
            Name = theEvent.Name;
        }

        #endregion Event Handlers

    }
}
