using MaginusLunch.Logging;
using MaginusLunch.Core.Extensions;
using MaginusLunch.Orders.Messages.Commands;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MaginusLunch.Orders.API.Authorization
{
    public class AddOrderCommandHandler : AuthorizationHandler<OrderCommandRequirement, AddOrder>
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(AddOrderCommandHandler));

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context, 
            OrderCommandRequirement requirement, 
            AddOrder resource)
        {
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
            if (!userIdClaim.Equals(resource.RecipientUserId, StringComparison.InvariantCultureIgnoreCase)
                && !requirement.AllowedNames.Any(name => userIdClaim.Equals(name, StringComparison.InvariantCultureIgnoreCase)))
            {
                _logger.WarnFormat("Failed to authorize User [{0}] with Claim of [{1}: {2}] as Recipient is [{3}].",
                    context.User.Identity.Name,
                    requirement.ClaimType,
                    emailAddress,
                    resource.RecipientUserId);
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
