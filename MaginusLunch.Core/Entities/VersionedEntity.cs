using System;

namespace MaginusLunch.Core.Entities
{
    public abstract class VersionedEntity : Entity
    {
        protected VersionedEntity(Guid id) : base(id) { }

        public int Version { get; set; }
    }
}
