using MaginusLunch.Core.Entities;
using System;
using System.Collections.Generic;

namespace MaginusLunch.Menu.Domain
{
    public class MenuOption : VersionedEntity
    {
        public MenuOption(Guid id) : base(id)
        {
            WithdrawnDates = new DateTimeHashSet();
            Status = MenuOptionStatus.Available;
        }

        public string Name { get; set; }
        public Guid LunchProviderId { get; set; }
        public ISet<DateTime> WithdrawnDates { get; private set; }
        public MenuOptionStatus Status { get; set; }

        public override object Clone()
        {
            var clone = MemberwiseClone() as MenuOption;
            clone.WithdrawnDates = new DateTimeHashSet(WithdrawnDates);
            return clone;
        }
    }
}
