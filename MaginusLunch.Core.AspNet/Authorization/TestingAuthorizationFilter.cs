using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Threading.Tasks;

namespace MaginusLunch.Core.AspNet.Authorization
{
    public class TestingAuthorizationFilterSettings
    {
        public const string DefaultTestingAuthorizationFilterSectionName = "TestingAuthorizationFilter";
        public string EmailClaimValue { get; set; }
        public bool Enabled { get; set; }
    }

    public class TestingAuthorizationFilter : IAsyncAuthorizationFilter
    {
        public TestingAuthorizationFilter(string emailClaimValue)
        {
            EmailClaimValue = emailClaimValue;
        }

        public string EmailClaimValue { get; }
        public Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            if (user.Identity == null
                || user.Claims == null
                || !user.Claims.Any()
                || !user.HasClaim(c => c.Type == "email"))
            {
                user.AddIdentity(new System.Security.Claims.ClaimsIdentity(new System.Security.Claims.Claim[] { new System.Security.Claims.Claim("email", EmailClaimValue) }));
            }
            return Task.CompletedTask;
        }
    }
}
