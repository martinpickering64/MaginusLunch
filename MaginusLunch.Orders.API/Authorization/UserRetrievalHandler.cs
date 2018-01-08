using MaginusLunch.Logging;
using MaginusLunch.Core.Extensions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MaginusLunch.Orders.API.Authorization
{
    public class UserRetrievalHandler : AuthorizationHandler<UserRetrievalRequirement, UserRetrievalAuthorizationResource>
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(AddOrderCommandHandler));

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context, 
            UserRetrievalRequirement requirement, 
            UserRetrievalAuthorizationResource resource)
        {
            if (resource == null)
            {
                _logger.WarnFormat("Unexpected attempt to authorize User [{0}] for the resource as the resource has not been specified - skipping decision.",
                                    context.User.Identity.Name);
                return Task.CompletedTask;
            }
            if (!context.User.HasClaim(c => c.Type == requirement.ClaimType))
            {
                _logger.WarnFormat("Failed to authorize User [{0}] due to an absence of a Claim of type [{1}].",
                    context.User.Identity.Name,
                    requirement.ClaimType);
                return Task.CompletedTask;
            }
            var emailAddress = context.User.FindFirst(c => c.Type == requirement.ClaimType).Value;
            if (string.IsNullOrWhiteSpace(emailAddress))
            {
                _logger.WarnFormat("Failed to authorize User [{0}] with Claim of [{1}] as its value is empty.",
                    context.User.Identity.Name,
                    requirement.ClaimType);
                return Task.CompletedTask;
            }
            var userIdClaim = emailAddress.EmailAddressToUserId();
            if (!userIdClaim.Equals(resource.UserId, StringComparison.InvariantCultureIgnoreCase)
                && !requirement.AllowedNames.Any(name => userIdClaim.Equals(name, StringComparison.InvariantCultureIgnoreCase)))
            {
                _logger.WarnFormat("Failed to authorize User [{0}] with Claim of [{1}: {2}] as Recipient is [{3}].",
                    context.User.Identity.Name,
                    requirement.ClaimType,
                    emailAddress,
                    resource.UserId);
                return Task.CompletedTask;
            }
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
    }
}
