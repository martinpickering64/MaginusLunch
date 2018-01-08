using MaginusLunch.Core.Entities;
using System;
using System.Collections.Generic;

namespace MaginusLunch.Orders.Domain
{
    public class LunchProvider : VersionedEntity
    {
        public LunchProvider(Guid id) : base(id)
        {
            WithdrawnDates = new DateTimeHashSet();
            Status = LunchProviderStatus.Available;
        }

        public string Name { get; set; }
        public ISet<DateTime> WithdrawnDates { get; private set; }
        public LunchProviderStatus Status { get; set; }

        public override object Clone()
        {
            var clone = MemberwiseClone() as LunchProvider;
            clone.WithdrawnDates = new DateTimeHashSet(WithdrawnDates);
            return clone;
        }
    }
}
