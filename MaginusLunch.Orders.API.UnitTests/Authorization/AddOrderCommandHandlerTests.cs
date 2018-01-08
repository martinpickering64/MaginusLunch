using MaginusLunch.Core.Logging;
using MaginusLunch.Logging;
using MaginusLunch.Orders.API.Authorization;
using MaginusLunch.Orders.Messages.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Security.Claims;

namespace MaginusLunch.Orders.API.UnitTests.Authorization
{
    [TestCategory("Unit Tests")]
    [TestClass]
    public class AddOrderCommandHandlerTests
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
        public void When_user_has_no_claims_does_not_authorize()
        {
            var resource = new AddOrder
            {
                Id = Guid.NewGuid(),
                RecipientUserId = "someone else",
                DeliveryDate = DateTime.UtcNow
            };
            var context = new AuthorizationHandlerContext(
                new[] { new OrderCommandRequirement() },
                new ClaimsPrincipal(new ClaimsIdentity(new Claim[0])),
                resource);
            var handler = new AddOrderCommandHandler();

            handler.HandleAsync(context).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.AreEqual(false, context.HasSucceeded);
            Assert.AreEqual(1, UnitTestingLogger.LogMsgs.Where(msg => msg.StartsWith("WARN")).Count());
        }

        [TestMethod]
        public void When_user_has_no_email_claim_does_not_authorize()
        {
            var resource = new AddOrder
            {
                Id = Guid.NewGuid(),
                RecipientUserId = "someone else",
                DeliveryDate = DateTime.UtcNow
            };
            var context = new AuthorizationHandlerContext(
                new[] { new OrderCommandRequirement() },
                new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("aClaimType", "aClaimValue") })),
                resource);
            var handler = new AddOrderCommandHandler();

            handler.HandleAsync(context).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.AreEqual(false, context.HasSucceeded);
            Assert.AreEqual(1, UnitTestingLogger.LogMsgs.Where(msg => msg.StartsWith("WARN")).Count());
        }

        [TestMethod]
        public void When_user_has_empty_email_claim_does_not_authorize()
        {
            var resource = new AddOrder
            {
                Id = Guid.NewGuid(),
                RecipientUserId = "someone else",
                DeliveryDate = DateTime.UtcNow
            };
            var requirement = new OrderCommandRequirement();
            var context = new AuthorizationHandlerContext(
                new[] { requirement },
                new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(requirement.ClaimType, string.Empty) })),
                resource);
            var handler = new AddOrderCommandHandler();

            handler.HandleAsync(context).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.AreEqual(false, context.HasSucceeded);
            Assert.AreEqual(1, UnitTestingLogger.LogMsgs.Where(msg => msg.StartsWith("WARN")).Count());
        }

        [TestMethod]
        public void When_user_not_housekeeper_cannot_order_onbehalf_of_another()
        {
            var requirement = new OrderCommandRequirement();
            var resource = new AddOrder
            {
                Id = Guid.NewGuid(),
                RecipientUserId = "someone else",
                DeliveryDate = DateTime.UtcNow
            };
            var context = new AuthorizationHandlerContext(
                new[] { requirement },
                new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(requirement.ClaimType, "not a houskeeper") })),
                resource);
            var handler = new AddOrderCommandHandler();

            handler.HandleAsync(context).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.AreEqual(false, context.HasSucceeded);
            Assert.AreEqual(1, UnitTestingLogger.LogMsgs.Where(msg => msg.StartsWith("WARN")).Count());
        }

        [TestMethod]
        public void When_user_is_housekeeper_can_order_onbehalf_of_another()
        {
            var requirement = new OrderCommandRequirement();
            var resource = new AddOrder
            {
                Id = Guid.NewGuid(),
                RecipientUserId = "someone else",
                DeliveryDate = DateTime.UtcNow
            };
            var context = new AuthorizationHandlerContext(
                new[] { requirement },
                new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(requirement.ClaimType, "martin.pickering") })),
                resource);
            var handler = new AddOrderCommandHandler();

            handler.HandleAsync(context).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.AreEqual(true, context.HasSucceeded);
            Assert.AreEqual(1, UnitTestingLogger.LogMsgs.Where(msg => msg.StartsWith("DEBUG")).Count());
        }

        [TestMethod]
        public void When_user_has_email_claim_can_order_for_themselves()
        {
            var requirement = new OrderCommandRequirement();
            var resource = new AddOrder
            {
                Id = Guid.NewGuid(),
                RecipientUserId = "me",
                DeliveryDate = DateTime.UtcNow
            };
            var context = new AuthorizationHandlerContext(
                new[] { requirement },
                new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(requirement.ClaimType, resource.RecipientUserId) })),
                resource);
            var handler = new AddOrderCommandHandler();

            handler.HandleAsync(context).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.AreEqual(true, context.HasSucceeded);
            Assert.AreEqual(1, UnitTestingLogger.LogMsgs.Where(msg => msg.StartsWith("DEBUG")).Count());
        }
    }
}
