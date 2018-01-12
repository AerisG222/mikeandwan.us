using System.Collections.Generic;
using IdentityServer4.Models;
using IdentityServer4;
using IdentityModel;

namespace MawAuth
{
    public class Config
    {
        // scopes define the API resources in your system
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("admin", "Administration APIs"),
                new ApiResource("blog", "Blog APIs"),
                new ApiResource("photo", "Photo APIs"),
                new ApiResource("video", "Video APIs")
            };
        }


        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResource("roles", "mikeandwan.us Roles", new[] { JwtClaimTypes.Role }),
            };
        }


        // clients want to access resources (aka scopes)
        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "www.mikeandwan.us",
                    ClientName = "www.mikeandwan.us",
                    AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,
                    RequireConsent = false,

                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },

                    // where to redirect to after login
                    RedirectUris = { "https://localhost:5021/signin-oidc" },

                    // where to redirect to after logout
                    PostLogoutRedirectUris = { "https://localhost:5021/signout-callback-oidc" },

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,

                        // apis
                        "admin",
                        "blog",
                        "photo",
                        "video",

                        // identity resources
                        "roles"
                    },
                    AllowOfflineAccess = true
                }
            };
        }
    }
}
