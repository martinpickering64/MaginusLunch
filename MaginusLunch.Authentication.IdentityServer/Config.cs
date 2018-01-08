using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace MaginusLunch.Authentication.IdentityServer
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId() { Required = true },
                new IdentityResources.Profile() {Required = true },
                new IdentityResources.Email() { Required = true },
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("MaginusLunch.Orders", "Maginus Lunch Ordering API")
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            // client credentials client
            return new List<Client>
            {
                new Client
                {
                    ClientId = "IntegrationTestClient",
                    // no interactive user, use the clientid/secret for authentication
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    // secret for authentication
                    ClientSecrets =
                    {
                        new Secret("IntegrationTest".Sha256())
                    },
                    // scopes that client has access to
                    AllowedScopes = {
                       "MaginusLunch.Orders"
                    }
                },
                // OpenID Connect hybrid flow and client credentials client (MVC)
                new Client
                {
                    ClientId = "LunchOrdering",
                    ClientName = "Maginus Lunch Ordering Client",
                    AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,

                    RequireConsent = true,

                    EnableLocalLogin = true,

                    ClientSecrets =
                    {
                        new Secret("albatross26TR".Sha256())
                    },

                    RedirectUris = { "https://localhost:44357/signin-oidc" },
                    PostLogoutRedirectUris = { "https://localhost:44357/signout-callback-oidc" },

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "MaginusLunch.Orders"
                    },
                    AllowOfflineAccess = true
                }
            };
        }
    }
}
