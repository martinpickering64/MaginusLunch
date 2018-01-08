namespace MaginusLunch.Core.Entities.UnitTests
{
    using System;
    using MaginusLunch.Core.Entities;

    public class TestEntity : VersionedEntity
    {
        public TestEntity(Guid id) : base(id)
        {}

        public override object Clone()
        {
            return MemberwiseClone() as TestEntity;
        }
    }
}
