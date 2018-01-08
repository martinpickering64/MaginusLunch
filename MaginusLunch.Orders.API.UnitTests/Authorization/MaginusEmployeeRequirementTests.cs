using MaginusLunch.Orders.API.Authorization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security.Claims;

namespace MaginusLunch.Orders.API.UnitTests.Authorization
{
    [TestCategory("Unit Tests")]
    [TestClass]
    public class MaginusEmployeeRequirementTests
    {
        [TestMethod]
        public void When_constructed_has_default_values()
        {
            var actual = new Core.AspNet.Authorization.MaginusEmployeeRequirement();

            Assert.AreEqual("email", actual.ClaimType);
            Assert.AreEqual("maginus.com", actual.Domain);
        }

        [TestMethod]
        public void When_constructed_can_overide_domain()
        {
            const string newDomain = "disney.com";
            var actual = new Core.AspNet.Authorization.MaginusEmployeeRequirement(newDomain);

            Assert.AreEqual("email", actual.ClaimType);
            Assert.AreEqual(newDomain, actual.Domain);
        }

        [TestMethod]
        public void When_constructed_can_overide_domain_and_claimType()
        {
            const string newDomain = "disney.com";
            var actual = new Core.AspNet.Authorization.MaginusEmployeeRequirement(newDomain, ClaimTypes.Email);

            Assert.AreEqual(ClaimTypes.Email, actual.ClaimType);
            Assert.AreEqual(newDomain, actual.Domain);
        }
    }
}
