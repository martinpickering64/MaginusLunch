using MaginusLunch.Orders.API.Authorization;
using MaginusLunch.Orders.API.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MaginusLunch.Orders.API.UnitTests
{
    [TestCategory("Unit Tests")]
    [TestClass]
    public class BasicIdentityTests
    {

        [TestMethod]
        public void WhenCallingApiWithNoClaimsPrinciple()
        {
            var authServiceMoq = new Mock<IAuthorizationService>();
            authServiceMoq.Setup(x => x.AuthorizeAsync(
                It.IsAny<ClaimsPrincipal>(), null, It.Is<string>(p => p.Equals("IsMaginusEmployee"))))
                .ReturnsAsync(AuthorizationResult.Failed());
            var identityController = new IdentityController(authorizationService: authServiceMoq.Object);

            var actionResult = identityController.Get().ConfigureAwait(false).GetAwaiter().GetResult();
            var challengeResult = actionResult as ChallengeResult;

            Assert.IsNotNull(challengeResult);
        }

        [TestMethod]
        public void WhenCallingApiWithaClaimsPrincipleButNoClaims()
        {
            var claimsIdentity = new ClaimsIdentity();
            var user = new ClaimsPrincipal(claimsIdentity);
            var authServiceMoq = new Mock<IAuthorizationService>();
            authServiceMoq.Setup<Task<AuthorizationResult>>(x => x.AuthorizeAsync(
                It.Is<ClaimsPrincipal>(p => p.Equals(user)), null, It.Is<string>(p => p.Equals("IsMaginusEmployee"))))
                .ReturnsAsync(RunAuthorizationHandler(user));
            var identityController = new IdentityController(authorizationService: authServiceMoq.Object);
            var httpContext = new DefaultHttpContext();
            identityController.ControllerContext.HttpContext = httpContext;
            identityController.ControllerContext.HttpContext.User = user;

            var actionResult = identityController.Get().ConfigureAwait(false).GetAwaiter().GetResult();

            var challengeResult = actionResult as ChallengeResult;
            Assert.IsNotNull(challengeResult);
        }

        [TestMethod]
        public void WhenCallingApiWithaClaimsPrincipleWithWrongEmailClaim()
        {
            var claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaim(new Claim("email", "martin@wrongDomain.com"));
            var user = new ClaimsPrincipal(claimsIdentity);
            var authServiceMoq = new Mock<IAuthorizationService>();
            authServiceMoq.Setup<Task<AuthorizationResult>>(x => x.AuthorizeAsync(
                It.Is<ClaimsPrincipal>(p => p.Equals(user)), null, It.Is<string>(p => p.Equals("IsMaginusEmployee"))))
                .ReturnsAsync(RunAuthorizationHandler(user));
            var identityController = new IdentityController(authorizationService: authServiceMoq.Object);
            var httpContext = new DefaultHttpContext();
            identityController.ControllerContext.HttpContext = httpContext;
            identityController.ControllerContext.HttpContext.User = user;

            var actionResult = identityController.Get().ConfigureAwait(false).GetAwaiter().GetResult();

            var challengeResult = actionResult as ChallengeResult;
            Assert.IsNotNull(challengeResult);
        }

        [TestMethod]
        public void WhenCallingApiWithaClaimsPrincipleWithAcceptableEmailClaim()
        {
            var claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaim(new Claim("email", "martin@maginus.com"));
            var user = new ClaimsPrincipal(claimsIdentity);
            var authServiceMoq = new Mock<IAuthorizationService>();
            authServiceMoq.Setup<Task<AuthorizationResult>>(x => x.AuthorizeAsync(
                It.Is<ClaimsPrincipal>(p => p.Equals(user)), null, It.Is<string>(p => p.Equals("IsMaginusEmployee"))))
                .ReturnsAsync(RunAuthorizationHandler(user));
            var identityController = new IdentityController(authorizationService: authServiceMoq.Object);
            var httpContext = new DefaultHttpContext();
            identityController.ControllerContext.HttpContext = httpContext;
            identityController.ControllerContext.HttpContext.User = user;

            var actionResult = identityController.Get().ConfigureAwait(false).GetAwaiter().GetResult();

            var jsonResult = actionResult as JsonResult;
            Assert.IsNotNull(jsonResult, $"actionResult is not a JsonResult. It is a {actionResult.GetType().Name}");
            var claims = jsonResult.Value as IEnumerable<Claim>;
            Assert.IsNotNull(claims, $"jsonResult.Value is not a IEnumerable<Claims>. It is a {jsonResult.Value.GetType().Name}");
            Assert.IsTrue(claims.Any(c => c.Type == "email"));
        }

        private AuthorizationResult RunAuthorizationHandler(ClaimsPrincipal user)
        {
            var handler = new Core.AspNet.Authorization.MaginusEmployeeHandler();
            var requirement = new Core.AspNet.Authorization.MaginusEmployeeRequirement();
            var context = new AuthorizationHandlerContext(new[] { requirement }, user, null);
            handler.HandleAsync(context).ConfigureAwait(false).GetAwaiter().GetResult();
            if (context.HasSucceeded)
            {
                return AuthorizationResult.Success();
            }
            return AuthorizationResult.Failed();

        }
    }
}
