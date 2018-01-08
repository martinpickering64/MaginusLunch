using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace MaginusLunch.Orders.API.Authorization
{
    public class UserRetrievalRequirement : IAuthorizationRequirement
    {
        private readonly OrderCommandRequirement _fiddle = new OrderCommandRequirement();

        public string ClaimType { get { return _fiddle.ClaimType; } }
        public IEnumerable<string> AllowedNames { get { return _fiddle.AllowedNames; } }

    }
}
