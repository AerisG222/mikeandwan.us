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
                new ApiResource("admin", "Administration API"),
                new ApiResource("blog", "Blog API"),
                new ApiResource("photo", "Photo API"),
                new ApiResource("video", "Video API", new [] { JwtClaimTypes.Name, JwtClaimTypes.Role })
            };
        }


        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResource(JwtClaimTypes.Role, "mikeandwan.us Roles", new[] { JwtClaimTypes.Role }),
            };
        }


        // clients want to access resources (aka scopes)
        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "maw_videos",
                    ClientName = "mikeandwan.us Video Application",
                    RequireConsent = false,
                    //AccessTokenLifetime = 600, // 10 minutes, default 60 minutes
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,
                    RedirectUris = new List<string>
                    {
                        "https://localhost:5021/videos/signin-oidc"
                    },
                    PostLogoutRedirectUris = new List<string>
                    {
                        "https://localhost:5021/"
                    },
                    AllowedCorsOrigins = new List<string>
                    {
                        "https://localhost:5021"
                    },
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,

                        // apis
                        "video",

                        // identity resources
                        JwtClaimTypes.Role
                    }
                },
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
                        JwtClaimTypes.Role
                    },
                    AllowOfflineAccess = true
                }
            };
        }
    }
}
