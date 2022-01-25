using Duende.IdentityServer.Models;
using Duende.IdentityServer;
using IdentityModel;

namespace MawAuth.Models;

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
            new ApiResource("maw_api_resource", "APIs to access photo and video data within mikeandwan.us") {
                Scopes = new string[] { "maw_api" }
            }
        };
    }

    // scopes define the API resources in your system
    public IEnumerable<ApiScope> GetApiScopes()
    {
        return new List<ApiScope>
        {
            new ApiScope("maw_api",
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
                AllowedGrantTypes = GrantTypes.Code,
                RequireConsent = false,

                ClientSecrets =
                {
                    new Secret(_wwwSecret.Sha256())
                },

                RedirectUris = { $"{_wwwUrl}/signin-oidc" },
                PostLogoutRedirectUris = { $"{_wwwUrl}/signout-callback-oidc" },
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
                ClientId = "maw_photos_android",
                ClientName = "MaW Photos - Android",
                AllowedGrantTypes = GrantTypes.Code,
                RequireClientSecret = false,
                AllowOfflineAccess = true,
                RedirectUris = { "us.mikeandwan.photos:/signin-oidc" },
                RefreshTokenUsage = TokenUsage.ReUse,
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
                }
            },
            new Client
            {
                ClientId = "maw-photos",
                ClientName = "mikeandwan.us Photo Application",
                AllowedGrantTypes = GrantTypes.Code,
                RequireClientSecret = false,
                AllowAccessTokensViaBrowser = true,
                AllowOfflineAccess = true,
                RedirectUris = new List<string>
                {
                    $"{_photosUrl}/login",
                    $"{_photosUrl}/silent-refresh.html"
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
                    IdentityServerConstants.StandardScopes.OfflineAccess,
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
                AllowedGrantTypes = GrantTypes.Code,
                RequireClientSecret = false,
                AllowAccessTokensViaBrowser = true,
                AllowOfflineAccess = true,
                RedirectUris = new List<string>
                {
                    $"{_filesUrl}/login",
                    $"{_filesUrl}/silent-refresh.html"
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
                    IdentityServerConstants.StandardScopes.OfflineAccess,
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
