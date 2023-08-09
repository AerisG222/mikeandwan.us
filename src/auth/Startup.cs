using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Authentication.Twitter;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.DataProtection;
using AspNet.Security.OAuth.GitHub;
using Duende.IdentityServer;
using Mvc.RenderViewToString;
using NWebsec.Core.Common.Middleware.Options;
using Maw.Data;
using Maw.Domain;
using Maw.Domain.Email;
using Maw.Domain.Models.Identity;
using Maw.Security;
using Maw.TagHelpers;
using MawAuth.Models;
using MawAuth.Services;

namespace MawAuth;

public class Startup
{
    readonly IConfiguration _config;

    public Startup(IConfiguration config)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
    }

    public void ConfigureServices(IServiceCollection services)
    {
        var config = new Config(
            _config["Environment:WwwUrl"] ?? throw new InvalidOperationException("www url cannot be null!"),
            _config["Environment:WwwClientSecret"] ?? throw new InvalidOperationException("www client secret cannot be null!"),
            _config["Environment:PhotosUrl"] ?? throw new InvalidOperationException("photos url cannot be null!"),
            _config["Environment:FilesUrl"] ?? throw new InvalidOperationException("files url cannot be null!"),
            _config["Environment:PhotosSolidUrl"] ?? throw new InvalidOperationException("photos-solid url cannot be null!"));

        ConfigureDataProtection(services);

        services
            .Configure<IdentityOptions>(opts =>
                {
                    opts.Password.RequiredLength = 8;
                    opts.Password.RequiredUniqueChars = 6;
                })
            .Configure<GmailApiEmailConfig>(_config.GetSection("Gmail"))
            .ConfigureMawTagHelpers(opts => {
                opts.AuthUrl = AddTrailingSlash(_config["Environment:AuthUrl"] ?? throw new InvalidOperationException("auth url cannot be null!"));
                opts.WwwUrl = AddTrailingSlash(_config["Environment:WwwUrl"] ?? throw new InvalidOperationException("www url cannot be null!"));
            })
            .AddMawDataServices(_config["Environment:DbConnectionString"] ?? throw new InvalidOperationException("db connection string cannot be null!"))
            .AddMawDomainAuthServices()
            .AddMawIdentityServerServices(_config["Environment:IdsrvDbConnectionString"] ?? throw new InvalidOperationException("idsrv db connection string cannot be null!"))
            .AddTransient<RazorViewToStringRenderer>()
            .AddIdentity<MawUser, MawRole>()
                .AddDefaultTokenProviders()
                .Services
            .ConfigureApplicationCookie(opts => ConfigureCookieOptions(opts))
            .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddGitHub(opts => ConfigureGithubAuthOptions(opts))
                .AddGoogle(opts => ConfigureGoogleAuthOptions(opts))
                .AddMicrosoftAccount(opts => ConfigureMicrosoftAuthOptions(opts))
                .AddTwitter(opts => ConfigureTwitterAuthOptions(opts))
                .Services
            .AddIdentityServer(opts =>
                {
                    // we need to set this especially for dev otherwise the issuer becomes 10.0.2.2 when testing the android app
                    opts.IssuerUri = _config["Environment:AuthUrl"];
                })
                .AddMawIdentityServerKeyMaterial(_config["SigningCertDir"] ?? throw new InvalidOperationException("signing cert dir cannot be null!"))
                .AddInMemoryApiScopes(config.GetApiScopes())
                .AddInMemoryApiResources(config.GetApiResources())
                .AddInMemoryClients(config.GetClients())
                .AddInMemoryIdentityResources(config.GetIdentityResources())
                .AddAspNetIdentity<MawUser>()
                .AddProfileService<IdentityServerProfileService>()
                .Services
            .AddAuthorization(opts => MawPolicyBuilder.AddMawPolicies(opts))
            .AddCors(opts => ConfigureDefaultCorsPolicy(opts))
            .AddResponseCompression()
            .AddControllersWithViews();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app
                .UseExceptionHandler("/error/")
                .UseHsts(hsts => hsts.MaxAge(365 * 2).IncludeSubdomains().Preload());
        }

        app
            .UseXContentTypeOptions()
            .UseReferrerPolicy(opts => opts.StrictOriginWhenCrossOrigin())

            .UseStaticFiles()

            .UseResponseCompression()
            .UseNoCacheHttpHeaders()
            .UseXfo(xfo => xfo.Deny())
            .UseXXssProtection(opts => opts.EnabledWithBlockMode())
            .UseRedirectValidation(opts => opts.AllowedDestinations(GetAllowedRedirectUrls()))
            .UseCsp(DefineContentSecurityPolicy)

            .UseRouting()
            .UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            })
            .UseAuthentication()
            .UseAuthorization()
            .UseIdentityServer()
            .UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
    }

    void ConfigureDataProtection(IServiceCollection services)
    {
        var dpPath = _config["DataProtection:Path"];

        if(!string.IsNullOrWhiteSpace(dpPath))
        {
            services
                .AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(dpPath));
        }
    }

    void ConfigureCookieOptions(CookieAuthenticationOptions opts)
    {
        opts.AccessDeniedPath = "/account/access-denied";
        opts.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        opts.LoginPath = "/account/login";
        opts.LogoutPath = "/account/logout";
    }

    void ConfigureGithubAuthOptions(GitHubAuthenticationOptions opts)
    {
        opts.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
        opts.ClientId = _config["GitHub:ClientId"] ?? throw new InvalidOperationException("github client id cannot be null!");
        opts.ClientSecret = _config["GitHub:ClientSecret"] ?? throw new InvalidOperationException("github client secret cannot be null!");
        opts.Scope.Add("user:email");
    }

    void ConfigureGoogleAuthOptions(GoogleOptions opts)
    {
        // https://github.com/aspnet/AspNetCore/issues/6486
        opts.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
        opts.ClientId = _config["GooglePlus:ClientId"] ?? throw new InvalidOperationException("google client id cannot be null!");
        opts.ClientSecret = _config["GooglePlus:ClientSecret"] ?? throw new InvalidOperationException("google client secret cannot be null!");
        opts.UserInformationEndpoint = "https://www.googleapis.com/oauth2/v2/userinfo";
        opts.ClaimActions.Clear();
        opts.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
        opts.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
        opts.ClaimActions.MapJsonKey(ClaimTypes.GivenName, "given_name");
        opts.ClaimActions.MapJsonKey(ClaimTypes.Surname, "family_name");
        opts.ClaimActions.MapJsonKey("urn:google:profile", "link");
        opts.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
    }

    void ConfigureMicrosoftAuthOptions(MicrosoftAccountOptions opts)
    {
        opts.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
        opts.ClientId = _config["Microsoft:ApplicationId"] ?? throw new InvalidOperationException("microsoft application id cannot be null!");
        opts.ClientSecret = _config["Microsoft:Secret"] ?? throw new InvalidOperationException("microsoft secret cannot be null!");
    }

    void ConfigureTwitterAuthOptions(TwitterOptions opts)
    {
        opts.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
        opts.ConsumerKey = _config["Twitter:ConsumerKey"];
        opts.ConsumerSecret = _config["Twitter:ConsumerSecret"];
        opts.RetrieveUserDetails = true;
    }

    void ConfigureDefaultCorsPolicy(CorsOptions opts)
    {
        opts.AddDefaultPolicy(builder =>
        {
            builder
                .WithOrigins(GetCorsOrigins())
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
    }

    string[] GetCorsOrigins()
    {
        return new string[] {
            _config["Environment:PhotosUrl"] ?? throw new InvalidOperationException("photos url cannot be null!"),
            _config["Environment:FilesUrl"] ?? throw new InvalidOperationException("files url cannot be null!"),
            _config["Environment:PhotosSolidUrl"] ?? throw new InvalidOperationException("photos url cannot be null!"),
        };
    }

    string[] GetAllowedRedirectUrls()
    {
        return new string[] {
            AddTrailingSlash(_config["Environment:FilesUrl"] ?? throw new InvalidOperationException("files url cannot be null!")),
            AddTrailingSlash(_config["Environment:PhotosUrl"] ?? throw new InvalidOperationException("photos url cannot be null!")),
            AddTrailingSlash(_config["Environment:PhotosSolidUrl"] ?? throw new InvalidOperationException("photos-solid url cannot be null!")),
            AddTrailingSlash(_config["Environment:WwwUrl"] ?? throw new InvalidOperationException("www url cannot be null!")),
            "https://accounts.google.com/o/oauth2/",
            "https://login.microsoftonline.com/common/oauth2/",
            "https://github.com/login/oauth/",
            "https://api.twitter.com/oauth/",
            "us.mikeandwan.photos:/"
        };
    }

    void DefineContentSecurityPolicy(IFluentCspOptions csp)
    {
        var fontSources = new string[] {
            "https://fonts.gstatic.com"
        };

        var imageSources = new string[] {
            "data:"
        };

        var reportUris = new string[] {
            "https://mikeandwanus.report-uri.com/r/d/csp/enforce"
        };

        var scriptSources = new string[] {
            "https://cdn.jsdelivr.net",
            "https://stackpath.bootstrapcdn.com"
        };

        var styleSources = new string[] {
            "https://fonts.googleapis.com"
        };

        csp
            .DefaultSources(s => s.None())
            .FontSources(s => s.CustomSources(fontSources))
            .ImageSources(s => {
                s.Self();
                s.CustomSources(imageSources);
            })
            .ManifestSources(s => s.Self())
            .ObjectSources(s => s.None())
            .ReportUris(s => s.Uris(reportUris))
            .ScriptSources(s => {
                s.Self();
                s.UnsafeInline();  // needed by identityserver
                s.CustomSources(scriptSources);
            })
            .StyleSources(s => {
                s.Self();
                s.UnsafeInline();
                s.CustomSources(styleSources);
            });
    }

    string AddTrailingSlash(string val)
    {
        return val.EndsWith('/') ? val : $"{val}/";
    }
}
