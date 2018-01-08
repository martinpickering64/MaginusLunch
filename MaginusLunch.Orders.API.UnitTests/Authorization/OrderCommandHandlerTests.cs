using MaginusLunch.Core.Logging;
using MaginusLunch.Orders.API.Authorization;
using MaginusLunch.Orders.Domain;
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
    public class OrderCommandHandlerTests
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
        public void When_handler_wont_authorize_with_no_resource()
        {
            var context = new AuthorizationHandlerContext(
                new[] { new OrderCommandRequirement() },
                new ClaimsPrincipal(new ClaimsIdentity(new Claim[0])),
                (AddOrder)null);
            var handler = new OrderCommandHandler();

            handler.HandleAsync(context).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.AreEqual(false, context.HasSucceeded);
            Assert.AreEqual(0, UnitTestingLogger.LogMsgs.Count); // our code has not even been called!
        }

        [TestMethod]
        public void When_command_is_AddOrder_does_not_authorize()
        {
            var testOrder = new Order(Guid.NewGuid());
            var testCommand = new AddOrder();
            var resource = new OrderCommandAuthorizationResource(testOrder, testCommand);
            var context = new AuthorizationHandlerContext(
                new[] { new OrderCommandRequirement() },
                new ClaimsPrincipal(new ClaimsIdentity(new Claim[0])),
                resource);
            var handler = new OrderCommandHandler();

            handler.HandleAsync(context).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.AreEqual(false, context.HasSucceeded);
            Assert.AreEqual(1, UnitTestingLogger.LogMsgs.Where(msg => msg.StartsWith("DEBUG")).Count());
        }

        [TestMethod]
        public void When_user_has_no_claims_does_not_authorize()
        {
            var testOrder = new Order(Guid.NewGuid());
            var testCommand = new AddBreadToOrder();
            var resource = new OrderCommandAuthorizationResource(testOrder, testCommand);
            var context = new AuthorizationHandlerContext(
                new[] { new OrderCommandRequirement() },
                new ClaimsPrincipal(new ClaimsIdentity(new Claim[0])),
                resource);
            var handler = new OrderCommandHandler();

            handler.HandleAsync(context).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.AreEqual(false, context.HasSucceeded);
            Assert.AreEqual(1, UnitTestingLogger.LogMsgs.Where(msg => msg.StartsWith("WARN")).Count());
        }

        [TestMethod]
        public void When_user_has_no_email_claim_does_not_authorize()
        {
            var testOrder = new Order(Guid.NewGuid());
            var testCommand = new AddBreadToOrder();
            var resource = new OrderCommandAuthorizationResource(testOrder, testCommand);
            var context = new AuthorizationHandlerContext(
                new[] { new OrderCommandRequirement() },
                new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("aClaimType", "aClaimValue") })),
                resource);
            var handler = new OrderCommandHandler();

            handler.HandleAsync(context).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.AreEqual(false, context.HasSucceeded);
            Assert.AreEqual(1, UnitTestingLogger.LogMsgs.Where(msg => msg.StartsWith("WARN")).Count());
        }

        [TestMethod]
        public void When_user_has_empty_email_claim_does_not_authorize()
        {
            var testOrder = new Order(Guid.NewGuid());
            var testCommand = new AddBreadToOrder();
            var resource = new OrderCommandAuthorizationResource(testOrder, testCommand);
            var requirement = new OrderCommandRequirement();
            var context = new AuthorizationHandlerContext(
                new[] { requirement },
                new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(requirement.ClaimType, string.Empty) })),
                resource);
            var handler = new OrderCommandHandler();

            handler.HandleAsync(context).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.AreEqual(false, context.HasSucceeded);
            Assert.AreEqual(1, UnitTestingLogger.LogMsgs.Where(msg => msg.StartsWith("WARN")).Count());
        }

        [TestMethod]
        public void When_user_not_housekeeper_cannot_act_onbehalf_of_another()
        {
            var testOrder = new Order(Guid.NewGuid()) { RecipientUserId = "someone else" };
            var testCommand = new AddBreadToOrder();
            var resource = new OrderCommandAuthorizationResource(testOrder, testCommand);
            var requirement = new OrderCommandRequirement();
            var context = new AuthorizationHandlerContext(
                new[] { requirement },
                new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(requirement.ClaimType, "not a houskeeper") })),
                resource);
            var handler = new OrderCommandHandler();

            handler.HandleAsync(context).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.AreEqual(false, context.HasSucceeded);
            Assert.AreEqual(1, UnitTestingLogger.LogMsgs.Where(msg => msg.StartsWith("WARN")).Count());
        }

        [TestMethod]
        public void When_user_is_housekeeper_can_act_onbehalf_of_another()
        {
            var testOrder = new Order(Guid.NewGuid()) { RecipientUserId = "someone else" };
            var testCommand = new AddBreadToOrder();
            var resource = new OrderCommandAuthorizationResource(testOrder, testCommand);
            var requirement = new OrderCommandRequirement();
            var context = new AuthorizationHandlerContext(
                new[] { requirement },
                new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(requirement.ClaimType, "martin.pickering") })),
                resource);
            var handler = new OrderCommandHandler();

            handler.HandleAsync(context).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.AreEqual(true, context.HasSucceeded);
            Assert.AreEqual(1, UnitTestingLogger.LogMsgs.Where(msg => msg.StartsWith("DEBUG")).Count());
        }

        [TestMethod]
        public void When_user_has_email_claim_can_act_for_themselves()
        {
            var testOrder = new Order(Guid.NewGuid()) { RecipientUserId = "someone else" };
            var testCommand = new AddBreadToOrder();
            var resource = new OrderCommandAuthorizationResource(testOrder, testCommand);
            var requirement = new OrderCommandRequirement();
            var context = new AuthorizationHandlerContext(
                new[] { requirement },
                new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(requirement.ClaimType, resource.Order.RecipientUserId) })),
                resource);
            var handler = new OrderCommandHandler();

            handler.HandleAsync(context).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.AreEqual(true, context.HasSucceeded);
            Assert.AreEqual(1, UnitTestingLogger.LogMsgs.Where(msg => msg.StartsWith("DEBUG")).Count());
        }
    }
}
