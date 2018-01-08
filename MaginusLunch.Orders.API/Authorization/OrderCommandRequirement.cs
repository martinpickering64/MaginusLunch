using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace MaginusLunch.Orders.API.Authorization
{
    public class OrderCommandRequirement : IAuthorizationRequirement
    {
        private static readonly IEnumerable<string> _names = new[] { "maggie.sproston", "martin.pickering" };
        public string ClaimType { get { return "email"; } }
        public IEnumerable<string> AllowedNames { get { return _names; } }
    }
}
