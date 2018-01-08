namespace MaginusLunch.Core.Entities.UnitTests
{
    using MaginusLunch.Core.Entities;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;

    [TestCategory("Unit")]
    [TestClass]
    public class EntityTests
    {
        [TestMethod]
        public void TestIdIsSetWhenConstructed()
        {
            var expected = Guid.NewGuid();

            var actual = new TestEntity(expected);

            Assert.AreEqual(expected, actual.Id);
        }

        [TestMethod]
        public void TestToStringMethodProducesExpectedValue()
        {
            var id = Guid.NewGuid();
            var expected = $"{typeof(TestEntity).Name}:{id}";
            var sus = new TestEntity(id);

            var actual = sus.ToString();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestHashCodeGenerationForDifferentObjects()
        {
            var a = new TestEntity(Guid.NewGuid());
            var b = new TestEntity(Guid.NewGuid());

            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [TestMethod]
        public void TestHashCodeGenerationForSameObjects()
        {
            var a = new TestEntity(Guid.NewGuid());
            var b = new TestEntity(a.Id);

            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [TestMethod]
        public void TestCompareWithNullObject()
        {
            var a = new TestEntity(Guid.NewGuid());

            Assert.AreEqual(1, a.CompareTo((object)null));
        }

        [TestMethod]
        public void TestCompareWithNullEntity()
        {
            var a = new TestEntity(Guid.NewGuid());
            var b = new TestEntity(a.Id);

            Assert.AreEqual(1, a.CompareTo((Entity)null));
        }

        [TestMethod]
        public void TestCompareEqualObjects()
        {
            var a = new TestEntity(Guid.NewGuid());
            var b = new TestEntity(a.Id);

            Assert.AreEqual(0, a.CompareTo((object)b));
        }

        [TestMethod]
        public void TestCompareEqualEntities()
        {
            var a = new TestEntity(Guid.NewGuid());
            var b = new TestEntity(a.Id);

            Assert.AreEqual(0, a.CompareTo(b));
        }

        [TestMethod]
        public void TestCompareEntityLessThan()
        {
            var a = new TestEntity(new Guid("{438b40f9-0c34-4765-a169-35b27462a817}"));
            var b = new TestEntity(new Guid("{b9f87c46-2ea7-40b7-8e22-562108c1cbe2}"));

            Assert.IsTrue(a.CompareTo(b) < 0);
        }

        [TestMethod]
        public void TestCompareEntityGreaterThan()
        {
            var a = new TestEntity(new Guid("{b9f87c46-2ea7-40b7-8e22-562108c1cbe2}"));
            var b = new TestEntity(new Guid("{438b40f9-0c34-4765-a169-35b27462a817}"));

            Assert.IsTrue(a.CompareTo(b) > 0);
        }

        [TestMethod]
        public void TestCompareObjectLessThan()
        {
            var a = new TestEntity(new Guid("{438b40f9-0c34-4765-a169-35b27462a817}"));
            var b = new TestEntity(new Guid("{b9f87c46-2ea7-40b7-8e22-562108c1cbe2}"));

            Assert.IsTrue(a.CompareTo((object)b) < 0);
        }

        [TestMethod]
        public void TestCompareObjectGreaterThan()
        {
            var a = new TestEntity(new Guid("{b9f87c46-2ea7-40b7-8e22-562108c1cbe2}"));
            var b = new TestEntity(new Guid("{438b40f9-0c34-4765-a169-35b27462a817}"));

            Assert.IsTrue(a.CompareTo((object)b) > 0);
        }

        [TestMethod]
        public void TestCompareofNotAnEntity()
        {
            try
            {
                var a = new TestEntity(Guid.NewGuid());
                const string b = "I am not an Entity!";

                a.CompareTo(b);

                Assert.Fail("Entity should have thrown an ArgumentException");
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual("other", ex.ParamName);
            }
        }

        [TestMethod]
        public void TestEqualsWithObjects()
        {
            var a = new TestEntity(Guid.NewGuid());
            var b = new TestEntity(a.Id);

            Assert.IsTrue(a.Equals((object)b));
        }

        [TestMethod]
        public void TestEqualsWithEntities()
        {
            var a = new TestEntity(Guid.NewGuid());
            var b = new TestEntity(a.Id);

            Assert.IsTrue(a.Equals(b));
        }

        [TestMethod]
        public void TestNotEqualsWithObjects()
        {
            var a = new TestEntity(Guid.NewGuid());
            var b = new TestEntity(Guid.NewGuid());

            Assert.IsFalse(a.Equals((object)b));
        }

        [TestMethod]
        public void TestNotEqualsWithEntities()
        {
            var a = new TestEntity(Guid.NewGuid());
            var b = new TestEntity(Guid.NewGuid());

            Assert.IsFalse(a.Equals(b));
        }

        [TestMethod]
        public void TestStaticEqualsWithEntities()
        {
            var a = new TestEntity(Guid.NewGuid());
            var b = new TestEntity(a.Id);

            Assert.IsTrue(Entity.Equals(a, b));
        }

        [TestMethod]
        public void TestStaticNotEqualsWithEntities()
        {
            var a = new TestEntity(Guid.NewGuid());
            var b = new TestEntity(Guid.NewGuid());

            Assert.IsFalse(Entity.Equals(a, b));
        }

        [TestMethod]
        public void TestEqualsOperatorWithEntities()
        {
            var a = new TestEntity(Guid.NewGuid());
            var b = new TestEntity(a.Id);

            Assert.IsTrue(a == b);
        }

        [TestMethod]
        public void TestNotEqualsOperatorWithEntities()
        {
            var a = new TestEntity(Guid.NewGuid());
            var b = new TestEntity(Guid.NewGuid());

            Assert.IsTrue(a != b);
        }
    }
}
