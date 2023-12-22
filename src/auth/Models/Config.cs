using Duende.IdentityServer.Models;
using Duende.IdentityServer;
using IdentityModel;

namespace MawAuth.Models;

public class Config
{
    static readonly string[] RESOURCE_API_SCOPES = [
        "maw_api"
    ];

    readonly string _wwwUrl;
    readonly string _wwwSecret;
    readonly string _photosUrl;
    readonly string _filesUrl;
    readonly string _photosSolidUrl;

    public Config(string wwwUrl, string wwwSecret, string photosUrl, string filesUrl, string photosSolidUrl)
    {
        ArgumentNullException.ThrowIfNull(wwwUrl);
        ArgumentNullException.ThrowIfNull(wwwSecret);
        ArgumentNullException.ThrowIfNull(photosUrl);
        ArgumentNullException.ThrowIfNull(filesUrl);
        ArgumentNullException.ThrowIfNull(photosSolidUrl);

        _wwwUrl = wwwUrl;
        _wwwSecret = wwwSecret;
        _photosUrl = photosUrl;
        _filesUrl = filesUrl;
        _photosSolidUrl = photosSolidUrl;
    }

    // scopes define the API resources in your system
    public IEnumerable<ApiResource> GetApiResources()
    {
        return new List<ApiResource>
        {
            new ApiResource("maw_api_resource", "APIs to access photo and video data within mikeandwan.us") {
                Scopes = RESOURCE_API_SCOPES
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
                // IdentityTokenLifetime = 5,
                // AccessTokenLifetime = 5,
                // AbsoluteRefreshTokenLifetime = 20
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
                ClientId = "maw-photos-solid",
                ClientName = "mikeandwan.us Photo Application",
                AllowedGrantTypes = GrantTypes.Code,
                RequireClientSecret = false,
                AllowAccessTokensViaBrowser = true,
                AllowOfflineAccess = true,
                RedirectUris = new List<string>
                {
                    $"{_photosSolidUrl}/login/handle-response"
                },
                // PostLogoutRedirectUris = new List<string>
                // {
                //     $"{_photosUrl}/signout-callback.html"
                // },
                AllowedCorsOrigins = new List<string>
                {
                    _photosSolidUrl
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
