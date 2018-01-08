using MaginusLunch.Core.Entities;
using System;
using System.Collections.Generic;

namespace MaginusLunch.Menu.Domain
{
    public class Filling : VersionedEntity
    {
        public Filling(Guid id) : base(id)
        {
            WithdrawnDates = new DateTimeHashSet();
            Status = FillingStatus.Available;
            MealType = MealType.SaladBox;
            MenuCategories = new List<Guid>();
        }

        public string Name { get; set; }
        public MealType MealType { get; set; }
        public Guid LunchProviderId { get; set; }
        public ISet<DateTime> WithdrawnDates { get; private set; }
        public IList<Guid> MenuCategories { get; private set; }
        public bool DisallowBread { get; set; }
        public bool DisallowMenuOption { get; set; }
        public FillingStatus Status { get; set; }

        public override object Clone()
        {
            var clone = MemberwiseClone() as Filling;
            clone.MenuCategories = new List<Guid>(MenuCategories);
            clone.WithdrawnDates = new DateTimeHashSet(WithdrawnDates);
            return clone;
        }
    }
}
