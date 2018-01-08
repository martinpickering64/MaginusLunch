using MaginusLunch.Core;
using MaginusLunch.Core.Entities;
using System;
using System.Collections.Generic;

namespace MaginusLunch.Orders.Domain
{
    public class Bread : VersionedEntity
    {
        public Bread(Guid id) : base(id)
        {
            WithdrawnDates = new DateTimeHashSet();
            Status = BreadStatus.Available;
        }

        public string Name { get; set; }
        public Guid LunchProviderId { get; set; }
        public ISet<DateTime> WithdrawnDates { get; private set; }
        public BreadStatus Status { get; set; }

        public override object Clone()
        {
            var clone = MemberwiseClone() as Bread;
            clone.WithdrawnDates = new DateTimeHashSet(WithdrawnDates);
            return clone;
        }
    }


}
