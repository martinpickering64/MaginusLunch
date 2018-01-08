using MaginusLunch.Core.Aggregates;
using MaginusLunch.Core.Entities;
using MaginusLunch.Menu.Messages.Commands;
using MaginusLunch.Menu.Messages.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaginusLunch.Menu.Domain
{
    public class CalendarAggregate : AggregateRoot
    {
        public CalendarAggregate(Guid id) : base()
        {
            Id = id;
            AvailableDays = new bool[7];
            WithdrawnDates = new DateTimeHashSet();
        }

        #region Aggregate Properties

        public string EditorUserId { get; private set; }
        public bool[] AvailableDays { get; }
        public ISet<DateTime> WithdrawnDates { get; }
        public DateTime OrdersStillOpenBeyond { get; private set; }

        #endregion Aggregate Properties

        #region Command Behaviours

        public CalendarAggregate(CreateCalendar command)
            : this(command.Id)
        {
            RaiseEvent(new CalendarCreated(command.Id, command.EditorUserId));
        }

        public void ActivateForDate(ActivateCalendarForDate command)
        {
            RaiseEvent(new CalendarActivatedForDate(command.Id, command.AffectedDate, command.EditorUserId));
        }

        public void AmendOpenOrderDate(AmendOpenOrderDate command)
        {
            RaiseEvent(new OpenOrderDateAmended(command.Id, command.NewOpenOrderDate, command.EditorUserId));
        }

        public void CloseDay(CloseDayOnCalendar command)
        {
            RaiseEvent(new DayClosedOnCalendar(command.Id, command.AffectedDay, command.EditorUserId));
        }

        public void OpenDay(OpenDayOnCalendar command)
        {
            RaiseEvent(new DayOpenedOnCalendar(command.Id, command.AffectedDay, command.EditorUserId));
        }

        public void WithdrawForDate(WithdrawCalendarForDate command)
        {
            RaiseEvent(new CalendarDateWithdrawn(command.Id, command.AffectedDate, command.EditorUserId));
        }

        #endregion Command Behaviours

        #region Event Handlers

        public void Apply(CalendarCreated theEvent)
        {
            Id = theEvent.Id;
            EditorUserId = theEvent.EditorUserId;
            AvailableDays[(int)DayOfWeek.Monday] = true;
            AvailableDays[(int)DayOfWeek.Tuesday] = true;
            AvailableDays[(int)DayOfWeek.Wednesday] = true;
            AvailableDays[(int)DayOfWeek.Thursday] = true;
            AvailableDays[(int)DayOfWeek.Friday] = true;
            AvailableDays[(int)DayOfWeek.Saturday] = false;
            AvailableDays[(int)DayOfWeek.Sunday] = false;
        }

        public void Apply(CalendarActivatedForDate theEvent)
        {
            var theDate = theEvent.AffectedDate;
            if (WithdrawnDates.Contains(theDate))
            {
                WithdrawnDates.Remove(theDate);
            }
        }

        public void Apply(OpenOrderDateAmended theEvent)
        {
            OrdersStillOpenBeyond = theEvent.NewOpenOrderDate;
        }

        public void Apply(DayClosedOnCalendar theEvent)
        {
            AvailableDays[(int)theEvent.DayNowClosed] = false;
        }

        public void Apply(DayOpenedOnCalendar theEvent)
        {
            AvailableDays[(int)theEvent.DayNowOpen] = true;
        }

        public void Apply(CalendarDateWithdrawn theEvent)
        {
            var theDate = theEvent.AffectedDate;
            if (WithdrawnDates.Contains(theDate))
            {
                return;
            }
            WithdrawnDates.Add(theDate);
        }

        #endregion Event Handlers
    }
}
