using MaginusLunch.Orders.API.Authorization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Security.Claims;

namespace MaginusLunch.Orders.API.UnitTests.Authorization
{
    [TestCategory("Unit Tests")]
    [TestClass]
    public class OrderCommandRequirementTests
    {
        [TestMethod]
        public void When_constructed_has_default_values()
        {
            var actual = new OrderCommandRequirement();

            Assert.AreEqual("email", actual.ClaimType);
            Assert.AreEqual(2, actual.AllowedNames.Count());
            Assert.AreEqual("maggie.sproston", actual.AllowedNames.First());
            Assert.AreEqual("martin.pickering", actual.AllowedNames.Last());
        }
    }
}
