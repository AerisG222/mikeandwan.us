using System;
using System.Collections.Generic;
using IdentityServer4.Models;
using IdentityServer4;
using IdentityModel;


namespace MawAuth.Models
{
    public class Config
    {
        readonly string _wwwUrl;
        readonly string _wwwSecret;
        readonly string _photosUrl;
        readonly string _filesUrl;


        public Config(string wwwUrl, string wwwSecret, string photosUrl, string filesUrl)
        {
            _wwwUrl = wwwUrl ?? throw new ArgumentNullException(nameof(wwwUrl));
            _wwwSecret = wwwSecret ?? throw new ArgumentNullException(nameof(wwwSecret));
            _photosUrl = photosUrl ?? throw new ArgumentNullException(nameof(photosUrl));
            _filesUrl = filesUrl ?? throw new ArgumentNullException(nameof(filesUrl));
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
                        $"{_photosUrl}/callback.html",
                        $"{_photosUrl}/renew-callback.html"
                    },
                    PostLogoutRedirectUris = new List<string>
                    {
                        $"{_photosUrl}/signout-callback.html"
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
                new Client
                {
                    ClientId = "maw-files",
                    ClientName = "mikeandwan.us File Management Application",
                    RequireConsent = false,
                    //AccessTokenLifetime = 600, // 10 minutes, default 60 minutes
                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    RequireClientSecret = false,
                    AllowAccessTokensViaBrowser = true,
                    RedirectUris = new List<string>
                    {
                        $"{_filesUrl}/callback.html",
                        $"{_filesUrl}/renew-callback.html"
                    },
                    PostLogoutRedirectUris = new List<string>
                    {
                        $"{_filesUrl}/signout-callback.html"
                    },
                    AllowedCorsOrigins = new List<string>
                    {
                        _filesUrl
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
                }
            };
        }
    }
}
