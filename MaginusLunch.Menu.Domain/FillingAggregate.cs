using MaginusLunch.Core.Aggregates;
using MaginusLunch.Core.Entities;
using MaginusLunch.Menu.Messages.Commands;
using MaginusLunch.Menu.Messages.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MaginusLunch.Menu.Domain
{
    public class FillingAggregate : AggregateRoot
    {
        public FillingAggregate(Guid id)
            : base()
        {
            Id = id;
            WithdrawnDates = new DateTimeHashSet();
            MenuCategories = new List<Guid>();
            Status = FillingStatus.Available;
        }

        #region Aggregate Properties

        public string EditorUserId { get; private set; }
        public Guid LunchProviderId { get; private set; }
        public string Name { get; private set; }
        public MealType MealType { get; private set; }
        public ISet<DateTime> WithdrawnDates { get; }
        public IList<Guid> MenuCategories { get; private set; }
        public bool DisallowBread { get; private set; }
        public bool DisallowMenuOptions { get; private set; }
        public FillingStatus Status { get; private set; }

        #endregion Aggregate Properties

        #region Command Behaviours

        public FillingAggregate(CreateFilling command)
            : this(command.Id)
        {
            RaiseEvent(new FillingCreated(command.Id,
                                          command.Name,
                                          command.MealType,
                                          command.LunchProviderId,
                                          command.EditorUserId)
            {
                Status = FillingStatus.Available
            });
        }

        public void Activate(ActivateFilling command)
        {
            RaiseEvent(new FillingActivated(command.Id, command.EditorUserId));
        }

        public void ActivateForDate(ActivateFillingForDate command)
        {
            RaiseEvent(new FillingActivatedForDate(command.Id, command.AffectedDate, command.EditorUserId));
        }

        public void AllowBreadWithFilling(AllowBreadWithFilling command)
        {
            RaiseEvent(new FillingAllowsBread(command.Id, command.EditorUserId));
        }

        public void AllowMenuOptionsWithFilling(AllowMenuOptionsWithFilling command)
        {
            RaiseEvent(new FillingAllowsMenuOptions(command.Id, command.EditorUserId));
        }

        public void ChangeName(ChangeNameOfFilling command)
        {
            RaiseEvent(new FillingNameChanged(command.Id, command.NewName, command.EditorUserId));
        }

        public void DisallowBreadWithFilling(DisallowBreadWithFilling command)
        {
            RaiseEvent(new FillingDisallowsBread(command.Id, command.EditorUserId));
        }

        public void DisallowMenuOptionsWithFilling(DisallowMenuOptionsWithFilling command)
        {
            RaiseEvent(new FillingDisallowsMenuOptions(command.Id, command.EditorUserId));
        }

        public void Update(UpdateFilling command)
        {
            RaiseEvent(new FillingUpdated(command.Id, command.MealType, command.MenuCategories, command.EditorUserId));
        }

        public void Withdraw(WithdrawFilling command)
        {
            RaiseEvent(new FillingWithdrawn(command.Id, command.EditorUserId));
        }

        public void WithdrawForDate(WithdrawFillingForDate command)
        {
            RaiseEvent(new FillingWithdrawnForDate(command.Id, command.AffectedDate, command.EditorUserId));
        }

        #endregion Command Behaviours

        #region Event Handlers

        public void Apply(FillingCreated theEvent)
        {
            Id = theEvent.Id;
            EditorUserId = theEvent.EditorUserId;
            Name = theEvent.Name;
            MealType = theEvent.MealType;
            LunchProviderId = theEvent.LunchProviderId;
            Status = theEvent.Status;
        }

        public void Apply(FillingActivated theEvent)
        {
            Status = FillingStatus.Available;
        }

        public void Apply(FillingActivatedForDate theEvent)
        {
            var theDate = theEvent.ActivatedOn;
            if (WithdrawnDates.Contains(theDate))
            {
                WithdrawnDates.Remove(theDate);
            }
        }

        public void Apply(FillingAllowsBread theEvent)
        {
            DisallowBread = false;
        }

        public void Apply(FillingAllowsMenuOptions theEvent)
        {
            DisallowMenuOptions = false;
        }

        public void Apply(FillingNameChanged theEvent)
        {
            Name = theEvent.Name;
        }

        public void Apply(FillingDisallowsBread theEvent)
        {
            DisallowBread = true;
        }

        public void Apply(FillingDisallowsMenuOptions theEvent)
        {
            DisallowMenuOptions = true;
        }

        public void Apply(FillingUpdated theEvent)
        {
            MealType = theEvent.MealType;
            if (MenuCategories.Any())
            {
                if (theEvent.MenuCategories.Any())
                {
                    MenuCategories = new List<Guid>(theEvent.MenuCategories);
                }
                else
                {
                    MenuCategories = new List<Guid>();
                }
            }
        }

        public void Apply(FillingWithdrawn theEvent)
        {
            Status = FillingStatus.Withdrawn;
        }

        public void Apply(FillingWithdrawnForDate theEvent)
        {
            var theDate = theEvent.WithdrawnOn;
            if (WithdrawnDates.Contains(theDate))
            {
                return;
            }
            WithdrawnDates.Add(theDate);
        }

        #endregion Event Handlers
    }
}
