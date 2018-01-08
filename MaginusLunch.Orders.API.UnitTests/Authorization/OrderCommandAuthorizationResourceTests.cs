using MaginusLunch.Orders.API.Authorization;
using MaginusLunch.Orders.Domain;
using MaginusLunch.Orders.Messages.Commands;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace MaginusLunch.Orders.API.UnitTests.Authorization
{
    [TestCategory("Unit Tests")]
    [TestClass]
    public class OrderCommandAuthorizationResourceTests
    {
        [TestMethod]
        public void When_constructed_must_provide_an_order()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
               new OrderCommandAuthorizationResource(null, new AddOrder()));
        }

        [TestMethod]
        public void When_constructed_must_provide_a_command()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
               new OrderCommandAuthorizationResource(new Order(Guid.NewGuid()), null));
        }

        [TestMethod]
        public void When_constructed_properties_are_initialised()
        {
            var testOrder = new Order(Guid.NewGuid());
            var testCommand = new AddOrder { Id = testOrder.Id };
            var actual = new OrderCommandAuthorizationResource(testOrder, testCommand);

            Assert.AreEqual(testOrder.Id, actual.Order.Id);
            Assert.AreEqual(testOrder, actual.Order);
            Assert.AreEqual(testCommand.Id, ((AddOrder)actual.Command).Id);
            Assert.AreEqual(testCommand, actual.Command);
        }
    }
}
