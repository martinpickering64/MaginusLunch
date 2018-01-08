using MaginusLunch.Logging;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace MaginusLunch.Core.AspNet.Authorization
{
    /// <summary>
    /// This Handler either Succeeds or Fails. It does not allow other Handlers in the Policy
    /// to Succeed when this requirement is not met and cause the operation to be authorised,
    /// i.e. this is a mandatory requirement.
    /// </summary>
    public class MaginusEmployeeHandler : AuthorizationHandler<MaginusEmployeeRequirement>
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(MaginusEmployeeHandler));

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context, 
            MaginusEmployeeRequirement requirement)
        {
            if (!context.User.HasClaim(c => c.Type == requirement.ClaimType))
            {
                _logger.WarnFormat("Failed to authorize User [{0}] due to an absence of a Claim of type [{1}].",
                    context.User.Identity.Name,
                    requirement.ClaimType);
                context.Fail();
                return Task.CompletedTask;
            }
            var emailAddress = context.User.FindFirst(c => c.Type == requirement.ClaimType).Value;
            if (!string.IsNullOrWhiteSpace(emailAddress)
                && emailAddress.EndsWith(requirement.Domain))
            {
                if (_logger.IsDebugEnabled)
                {
                    _logger.DebugFormat("User [{0}] has been authorized with Claim [{1}: {2}].",
                        context.User.Identity.Name,
                        requirement.ClaimType,
                        emailAddress);
                }
                context.Succeed(requirement);
                return Task.CompletedTask;
            }
            _logger.WarnFormat("Failed to authorize User [{0}] with Claim of [{1}: {2}] for domain [{3}].", 
                context.User.Identity.Name, 
                requirement.ClaimType,
                emailAddress,
                requirement.Domain);
            context.Fail();
            return Task.CompletedTask;
        }
    }
}
