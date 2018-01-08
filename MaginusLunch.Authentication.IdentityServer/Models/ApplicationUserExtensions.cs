using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace MaginusLunch.Authentication.IdentityServer.Models
{
    public static class ApplicationUserExtensions
    {
        /// <summary>
        /// create the list of claims that we want to transfer into our store
        /// </summary>
        public static void AddExternalClaims(this ApplicationUser user, ClaimsPrincipal principal)
        {
            user.Claims.MapClaimsFromPrincipal(principal);
        }

        /// <summary>
        /// Maps external claims types to internal claim types and performs
        /// some sundry fix-ups
        /// </summary>
        /// <remarks>
        /// As this is Ap.Net Identity we can skip Email claims as they are handled
        /// directly by ASP.Net Identity.
        /// If we are working with Microsoft Accounts then it duplicates its claims
        /// using the SAML Claim Types and its own Claim Types using "urn:microsoftaccount:something", 
        /// so we can skip all those duplicates as well.
        /// </remarks>
        private static void MapClaimsFromPrincipal(this ICollection<IdentityUserClaim<string>> userClaims, ClaimsPrincipal principal)
        {
            foreach (var claim in principal.Claims
                                    .Where(c => !c.Type.Equals(ClaimTypes.Email)
                                                && !c.Type.Equals(JwtClaimTypes.Email)
                                                && !c.Type.StartsWith("urn:microsoftaccount:")))
            {
                // if the external system sends a display name - translate that to the standard OIDC name claim
                if (claim.Type == ClaimTypes.Name)
                {
                    userClaims.Add(new IdentityUserClaim<string> { ClaimType = JwtClaimTypes.Name, ClaimValue = claim.Value });
                }
                // if the JWT handler has an outbound mapping to an OIDC claim use that
                else if (JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.ContainsKey(claim.Type))
                {
                    userClaims.Add(new IdentityUserClaim<string> { ClaimType = JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap[claim.Type], ClaimValue = claim.Value });
                }
                // copy the claim as-is
                else
                {
                    userClaims.Add(new IdentityUserClaim<string> { ClaimType = claim.Type, ClaimValue = claim.Value });
                }
            }
            // if no display name was provided, try to construct by first and/or last name
            if (!userClaims.Any(x => x.ClaimType == JwtClaimTypes.Name))
            {
                var first = userClaims.FirstOrDefault(x => x.ClaimType == JwtClaimTypes.GivenName)?.ClaimValue;
                var last = userClaims.FirstOrDefault(x => x.ClaimType == JwtClaimTypes.FamilyName)?.ClaimValue;
                if (first != null && last != null)
                {
                    userClaims.Add(new IdentityUserClaim<string> { ClaimType = JwtClaimTypes.Name, ClaimValue = $"{first} {last}" });
                }
                else if (first != null)
                {
                    userClaims.Add(new IdentityUserClaim<string> { ClaimType = JwtClaimTypes.Name, ClaimValue = last });
                }
                else if (last != null)
                {
                    userClaims.Add(new IdentityUserClaim<string> { ClaimType = JwtClaimTypes.Name, ClaimValue = last });
                }
            }
        }

        /// <summary>
        /// Refresh the Claims list in our User Store based on their current values from
        /// the External Provider.
        /// </summary>
        public static async void RefreshExternalClaims(this UserManager<ApplicationUser> userManager, ExternalLoginInfo info)
        {
            if (!info.Principal.Claims.Any())
            {
                return;
            }
            var theUser = await userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey).ConfigureAwait(false);
            var currentExternalClaims = new List<IdentityUserClaim<string>>();
            currentExternalClaims.MapClaimsFromPrincipal(info.Principal);
            var userClaims = await userManager.GetClaimsAsync(theUser).ConfigureAwait(false);
            if (!userClaims.Any())
            {
                await userManager.AddClaimsAsync(theUser, currentExternalClaims.Select(exClaim => exClaim.ToClaim())).ConfigureAwait(false);
                return;
            }
            foreach (var claim in currentExternalClaims)
            {
                var userClaim = userClaims.FirstOrDefault(uc => uc.Type.Equals(claim.ClaimType));
                if (userClaim == null)
                {
                    await userManager.AddClaimAsync(theUser, claim.ToClaim()).ConfigureAwait(false);
                }
                else
                {
                    if (userClaim.Value != claim.ClaimValue)
                    {
                        await userManager.ReplaceClaimAsync(theUser, userClaim, claim.ToClaim()).ConfigureAwait(false);
                    }
                }
            }
        }
    }
}
