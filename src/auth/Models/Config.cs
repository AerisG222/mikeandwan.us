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
        readonly string _photosUrl;


        public Config(string wwwUrl, string wwwSecret, string photosUrl)
        {
            _wwwUrl = wwwUrl ?? throw new ArgumentNullException(nameof(wwwUrl));
            _wwwSecret = wwwSecret ?? throw new ArgumentNullException(nameof(wwwSecret));
            _photosUrl = photosUrl ?? throw new ArgumentNullException(nameof(photosUrl));
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
                    ClientId = "maw_upload",
                    ClientName = "mikeandwan.us Upload Application",
                    RequireConsent = false,
                    //AccessTokenLifetime = 600, // 10 minutes, default 60 minutes
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,
                    RedirectUris = new List<string>
                    {
                        $"{_wwwUrl}/upload/signin-oidc",
                        $"{_wwwUrl}/account/spa-silent-signin"
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
                    ClientId = "maw_videos",
                    ClientName = "mikeandwan.us Video Application",
                    RequireConsent = false,
                    //AccessTokenLifetime = 600, // 10 minutes, default 60 minutes
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,
                    RedirectUris = new List<string>
                    {
                        $"{_wwwUrl}/videos/signin-oidc",
                        $"{_wwwUrl}/account/spa-silent-signin"
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
                        $"{_wwwUrl}/photos/signin-oidc",
                        $"{_wwwUrl}/account/spa-silent-signin"
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
                        IdentityServerConstants.StandardScopes.Profile,

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
                        $"{_wwwUrl}/photos/stats/signin-oidc",
                        $"{_wwwUrl}/account/spa-silent-signin"
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
                        $"{_wwwUrl}/photos/3d/signin-oidc",
                        $"{_wwwUrl}/account/spa-silent-signin"
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
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,

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

                    // uncomment to test refresh token scenario in Android emulator
                    //AccessTokenLifetime = 30,

                    // where to redirect to after login
                    RedirectUris = { "us.mikeandwan.photos:/signin-oidc" },

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,

                        // apis
                        "maw_api",

                        // identity resources
                        JwtClaimTypes.Role
                    },
                    AllowOfflineAccess = true
                },
                new Client
                {
                    ClientId = "maw-photos",
                    ClientName = "NEW mikeandwan.us Photo Application",
                    RequireConsent = false,
                    //AccessTokenLifetime = 600, // 10 minutes, default 60 minutes
                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    RequireClientSecret = false,
                    AllowAccessTokensViaBrowser = true,
                    RedirectUris = new List<string>
                    {
                        $"{_photosUrl}/auth",
                        $"{_photosUrl}/auth/silent"
                    },
                    PostLogoutRedirectUris = new List<string>
                    {
                        $"{_photosUrl}/"
                    },
                    AllowedCorsOrigins = new List<string>
                    {
                        _photosUrl
                    },
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,

                        // apis
                        "maw_api",

                        // identity resources
                        JwtClaimTypes.Role
                    }
                },
            };
        }
    }
}
