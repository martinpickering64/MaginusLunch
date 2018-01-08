using MaginusLunch.Core.Entities;
using System;

namespace MaginusLunch.Menu.Domain
{
    public class MenuCategory : VersionedEntity
    {
        public MenuCategory(Guid id) : base(id)
        { }

        public string Name { get; set; }

        public override object Clone()
        {
            var clone = MemberwiseClone() as MenuCategory;
            return clone;
        }
    }
}
