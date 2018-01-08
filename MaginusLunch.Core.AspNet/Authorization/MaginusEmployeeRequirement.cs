using Microsoft.AspNetCore.Authorization;

namespace MaginusLunch.Core.AspNet.Authorization
{
    public class MaginusEmployeeRequirement : IAuthorizationRequirement
    {
        public MaginusEmployeeRequirement()
            : this("maginus.com")
        { }

        public MaginusEmployeeRequirement(string domain)
            : this(domain, "email")
        { }

        public MaginusEmployeeRequirement(string domain, string claimType)
        {
            Domain = domain;
            ClaimType = claimType;
        }

        public string Domain { get; }
        public string ClaimType { get; }

    }
}
