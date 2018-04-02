using System.Collections.Generic;
using IdentityServer4.Models;
using IdentityServer4;
using IdentityModel;
using System;


namespace MawAuth.Models
{
    public class Config
    {
        readonly string _wwwUrl;
        readonly string _wwwSecret;


        public Config(string wwwUrl, string wwwSecret)
        {
            _wwwUrl = wwwUrl ?? throw new ArgumentNullException(nameof(wwwUrl));
            _wwwSecret = wwwSecret ?? throw new ArgumentNullException(nameof(wwwSecret));
        }


        // scopes define the API resources in your system
        public IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("maw_api",
                                "APIs to access photo and video data within mikeandwan.us",
                                new [] { JwtClaimTypes.Name, JwtClaimTypes.Role })
            };
        }


        public IEnumerable<IdentityResource> GetIdentityResources()
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
        public IEnumerable<Client> GetClients()
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
                        $"{_wwwUrl}/videos/signin-oidc"
                    },
                    PostLogoutRedirectUris = new List<string>
                    {
                        $"{_wwwUrl}/"
                    },
                    AllowedCorsOrigins = new List<string>
                    {
                        _wwwUrl
                    },
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,

                        // apis
                        "maw_api",

                        // identity resources
                        JwtClaimTypes.Role
                    }
                },
                new Client
                {
                    ClientId = "maw_photos",
                    ClientName = "mikeandwan.us Photo Application",
                    RequireConsent = false,
                    //AccessTokenLifetime = 600, // 10 minutes, default 60 minutes
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,
                    RedirectUris = new List<string>
                    {
                        $"{_wwwUrl}/photos/signin-oidc"
                    },
                    PostLogoutRedirectUris = new List<string>
                    {
                        $"{_wwwUrl}/"
                    },
                    AllowedCorsOrigins = new List<string>
                    {
                        _wwwUrl
                    },
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,

                        // apis
                        "maw_api",

                        // identity resources
                        JwtClaimTypes.Role
                    }
                },
                new Client
                {
                    ClientId = "maw_photo_stats",
                    ClientName = "mikeandwan.us Photo Stats Application",
                    RequireConsent = false,
                    //AccessTokenLifetime = 600, // 10 minutes, default 60 minutes
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,
                    RedirectUris = new List<string>
                    {
                        $"{_wwwUrl}/photos/stats/signin-oidc"
                    },
                    PostLogoutRedirectUris = new List<string>
                    {
                        $"{_wwwUrl}/"
                    },
                    AllowedCorsOrigins = new List<string>
                    {
                        _wwwUrl
                    },
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,

                        // apis
                        "maw_api",

                        // identity resources
                        JwtClaimTypes.Role
                    }
                },
                new Client
                {
                    ClientId = "maw_photos_3d",
                    ClientName = "mikeandwan.us Photo 3D Application",
                    RequireConsent = false,
                    //AccessTokenLifetime = 600, // 10 minutes, default 60 minutes
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,
                    RedirectUris = new List<string>
                    {
                        $"{_wwwUrl}/photos/3d/signin-oidc"
                    },
                    PostLogoutRedirectUris = new List<string>
                    {
                        $"{_wwwUrl}/"
                    },
                    AllowedCorsOrigins = new List<string>
                    {
                        _wwwUrl
                    },
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,

                        // apis
                        "maw_api",

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
                        new Secret(_wwwSecret.Sha256())
                    },

                    // where to redirect to after login
                    RedirectUris = { $"{_wwwUrl}/signin-oidc" },

                    // where to redirect to after logout
                    PostLogoutRedirectUris = { $"{_wwwUrl}/signout-callback-oidc" },

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,

                        // apis
                        "maw_api",

                        // identity resources
                        JwtClaimTypes.Role
                    },
                    AllowOfflineAccess = true
                },
                new Client
                {
                    ClientId = "maw_photos_android",
                    ClientName = "MaW Photos - Android",
                    AllowedGrantTypes = GrantTypes.Code,
                    RequireConsent = false,
                    RequireClientSecret = false,
                    RequirePkce = true,

                    // where to redirect to after login
                    RedirectUris = { "us.mikeandwan.photos:/signin-oidc" },

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Email,

                        // apis
                        "maw_api",

                        // identity resources
                        JwtClaimTypes.Role
                    },
                    AllowOfflineAccess = true
                }
            };
        }
    }
}
