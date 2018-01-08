using MaginusLunch.Core.Logging;
using MaginusLunch.Orders.API.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Security.Claims;

namespace MaginusLunch.Orders.API.UnitTests.Authorization
{
    [TestCategory("Unit Tests")]
    [TestClass]
    public class MaginusEmployeeHandlerTests
    {
        [TestInitialize]
        public void TestSetup()
        {
            MaginusLunch.Logging.LogManager.Use<UnitTestLoggerFactory>();
        }

        [TestCleanup]
        public void TestTearDown()
        {
            UnitTestingLogger.LogMsgs.Clear();
        }

        [TestMethod]
        public void When_user_has_no_claims_fails_authorization()
        {
            var context = new AuthorizationHandlerContext(
                new[] { new Core.AspNet.Authorization.MaginusEmployeeRequirement()},
                new ClaimsPrincipal(new ClaimsIdentity(new Claim[0])),
                null);
            var handler = new Core.AspNet.Authorization.MaginusEmployeeHandler();

            handler.HandleAsync(context).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.AreEqual(true, context.HasFailed);
            Assert.AreEqual(1, UnitTestingLogger.LogMsgs.Where(msg => msg.StartsWith("WARN")).Count());
        }

        [TestMethod]
        public void When_user_has_no_email_claim_fails_authorization()
        {
            var context = new AuthorizationHandlerContext(
                new[] { new Core.AspNet.Authorization.MaginusEmployeeRequirement() },
                new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("aClaimType", "aClaimValue") })),
                null);
            var handler = new Core.AspNet.Authorization.MaginusEmployeeHandler();

            handler.HandleAsync(context).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.AreEqual(true, context.HasFailed);
            Assert.AreEqual(1, UnitTestingLogger.LogMsgs.Where(msg => msg.StartsWith("WARN")).Count());
        }

        [TestMethod]
        public void When_user_has_wrong_email_claim_fails_authorization()
        {
            var requirement = new Core.AspNet.Authorization.MaginusEmployeeRequirement();
            var context = new AuthorizationHandlerContext(
                new[] { requirement },
                new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(requirement.ClaimType, "martin@martinpickering.com") })),
                null);
            var handler = new Core.AspNet.Authorization.MaginusEmployeeHandler();

            handler.HandleAsync(context).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.AreEqual(true, context.HasFailed);
            Assert.AreEqual(1, UnitTestingLogger.LogMsgs.Where(msg => msg.StartsWith("WARN")).Count());
        }

        [TestMethod]
        public void When_user_has_correct_email_claim_is_authorized()
        {
            var requirement = new Core.AspNet.Authorization.MaginusEmployeeRequirement();
            var context = new AuthorizationHandlerContext(
                new[] { requirement },
                new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(requirement.ClaimType, string.Concat("martin@", requirement.Domain)) })),
                null);
            var handler = new Core.AspNet.Authorization.MaginusEmployeeHandler();

            handler.HandleAsync(context).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.AreEqual(true, context.HasSucceeded);
            Assert.AreEqual(1, UnitTestingLogger.LogMsgs.Where(msg => msg.StartsWith("DEBUG")).Count());
        }
    }
}
