using MaginusLunch.Core.Entities;
using System;
using System.Collections.Generic;

namespace MaginusLunch.Orders.Domain
{
    public class Calendar : VersionedEntity
    { 
        public Calendar(Guid id):base(id)
        {
            AvailableDays = new bool[]
            {
                false, true, true, true, true, true, false
                //{ DayOfWeek.Sunday, false }
                //{ DayOfWeek.Monday, true },
                //{ DayOfWeek.Tuesday, true },
                //{ DayOfWeek.Wednesday, true },
                //{ DayOfWeek.Thursday, true },
                //{ DayOfWeek.Friday, true },
                //{ DayOfWeek.Saturday, false },
            };

            WithdrawnDates = new DateTimeHashSet();
            SetOpenOrderDate(DateTime.UtcNow);
        }

        private void SetOpenOrderDate(DateTime desiredDate)
        {
            if (desiredDate.Kind != DateTimeKind.Utc)
            {
                desiredDate = desiredDate.ToUniversalTime();
            }
            var now = DateTime.UtcNow;
            if (now > desiredDate)
            {
                desiredDate = now;
            }
            if (now.Date == desiredDate.Date
                && now.TimeOfDay.Hours >= 10)
            {
                desiredDate.AddDays(1);
            }
            if (desiredDate.DayOfWeek == DayOfWeek.Saturday)
            {
                desiredDate.AddDays(2);
            }
            if (desiredDate.DayOfWeek == DayOfWeek.Sunday)
            {
                desiredDate.AddDays(1);
            }
            OrdersStillOpenBeyond = desiredDate;
        }

        public bool[] AvailableDays { get; private set; }
        public ISet<DateTime> WithdrawnDates { get; private set; }
        public DateTime OrdersStillOpenBeyond { get; set; }

        public override object Clone()
        {
            Calendar clone = (Calendar)MemberwiseClone();
            clone.WithdrawnDates = new DateTimeHashSet(WithdrawnDates);
            clone.AvailableDays = (bool[])AvailableDays.Clone();
            return clone;
        }
    }
}
