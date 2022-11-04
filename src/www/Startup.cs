using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Microsoft.FeatureManagement;
using IdentityModel;
using Mvc.RenderViewToString;
using NWebsec.Core.Common.Middleware.Options;
using Maw.Cache;
using Maw.Data;
using Maw.Domain;
using Maw.Domain.Captcha;
using Maw.Domain.Email;
using Maw.Security;
using Maw.TagHelpers;
using MawMvcApp.ViewModels;
using MawMvcApp.ViewModels.About;

namespace MawMvcApp;

public class Startup
{
    readonly IConfiguration _config;
    readonly IWebHostEnvironment _env;

    public Startup(IConfiguration config, IWebHostEnvironment hostingEnvironment)
    {
        _env = hostingEnvironment ?? throw new ArgumentNullException(nameof(hostingEnvironment));
        _config = config ?? throw new ArgumentNullException(nameof(config));
    }

    public void ConfigureServices(IServiceCollection services)
    {
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

        var authConfig = _config.GetSection("AuthConfig").Get<AuthConfig>();

        ConfigureDataProtection(services);

        services
            .Configure<ContactConfig>(_config.GetSection("ContactUs"))
            .Configure<GmailApiEmailConfig>(_config.GetSection("Gmail"))
            .Configure<EnvironmentConfig>(_config.GetSection("Environment"))
            .Configure<GoogleCaptchaConfig>(_config.GetSection("GoogleRecaptcha"))
            .Configure<CloudflareTurnstileConfig>(_config.GetSection("CloudflareTurnstile"))
            .Configure<UrlConfig>(_config.GetSection("UrlConfig"))
            .ConfigureMawTagHelpers(opts =>
            {
                opts.AuthUrl = AddTrailingSlash(_config["UrlConfig:Auth"]);
                opts.WwwUrl = AddTrailingSlash(_config["UrlConfig:Www"]);
            })
            .AddFeatureManagement()
                .Services
            .AddLogging()
            .AddHttpContextAccessor()
            .AddResponseCompression()
            .AddMawDataServices(_config["Environment:DbConnectionString"])
            .AddMawCacheServices(_config["Environment:RedisConnectionString"])
            .AddMawDomainServices()
            .AddTransient<RazorViewToStringRenderer>()
            .AddScoped<IContentTypeProvider, FileExtensionContentTypeProvider>()
            .AddSingleton<IFileProvider>(x => new PhysicalFileProvider(_config["Environment:AssetsPath"]))
            .AddAntiforgery(opts => opts.HeaderName = "X-XSRF-TOKEN")
            .AddAuthentication(opts => ConfigureAuthenticationOptions(opts))
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, opts => ConfigureCookieOptions(opts))
                .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, opts => ConfigureOidcOptions(opts, authConfig))
                .Services
            .AddAuthorization(opts => MawPolicyBuilder.AddMawPolicies(opts))
            .AddCors(opts => ConfigureDefaultCorsPolicy(opts))
            .AddControllersWithViews()
                .Services
            .AddRazorPages();

        if (_env.IsDevelopment())
        {
            services.AddMiniProfiler();
        }
    }

    public void Configure(IApplicationBuilder app)
    {
        if (_env.IsDevelopment())
        {
            app
                .UseMiniProfiler()
                .UseDeveloperExceptionPage();

            AddDevPathMappings(app);
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

            .UseCors()

            .UseStaticFiles(new StaticFileOptions
            {
                ContentTypeProvider = GetCustomMimeTypeProvider()
            })

            .UseResponseCompression()
            .UseNoCacheHttpHeaders()
            .UseXfo(xfo => xfo.SameOrigin())
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
            .UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });
    }

    void ConfigureDataProtection(IServiceCollection services)
    {
        var dpPath = _config["DataProtection:Path"];

        if (!string.IsNullOrWhiteSpace(dpPath))
        {
            services
                .AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(dpPath));
        }
    }

    void ConfigureAuthenticationOptions(AuthenticationOptions opts)
    {
        opts.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        opts.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    }

    void ConfigureCookieOptions(CookieAuthenticationOptions opts)
    {
        opts.AccessDeniedPath = "/account/access-denied";
        opts.LogoutPath = "/account/logout";
        opts.ExpireTimeSpan = TimeSpan.FromMinutes(15);
    }

    void ConfigureOidcOptions(OpenIdConnectOptions opts, AuthConfig authConfig)
    {
        opts.Authority = authConfig.AuthorizationUrl;

        opts.ClientId = authConfig.ClientId;
        opts.ClientSecret = authConfig.Secret;
        opts.ResponseType = "code";

        opts.SaveTokens = true;
        opts.GetClaimsFromUserInfoEndpoint = true;

        // core
        opts.Scope.Add("openid");
        opts.Scope.Add("profile");
        opts.Scope.Add("offline_access");

        // apis
        opts.Scope.Add("maw_api");

        // identity resources
        opts.Scope.Add(JwtClaimTypes.Role);
        opts.Scope.Add("email");

        // https://github.com/IdentityServer/IdentityServer4/issues/1786
        opts.ClaimActions.MapJsonKey("role", "role", "role");

        opts.TokenValidationParameters.NameClaimType = JwtClaimTypes.Name;
        opts.TokenValidationParameters.RoleClaimType = JwtClaimTypes.Role;
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
                _config["UrlConfig:Api"],
                _config["UrlConfig:Photos"]
            };
    }

    string[] GetAllowedRedirectUrls()
    {
        return new string[] {
                AddTrailingSlash(_config["UrlConfig:Auth"]),
                AddTrailingSlash(_config["UrlConfig:Files"]),
                AddTrailingSlash(_config["UrlConfig:Photos"])
            };
    }

    void DefineContentSecurityPolicy(IFluentCspOptions csp)
    {
        // https://developers.google.com/maps/documentation/javascript/content-security-policy
        var connectSources = new string[] {
                "https://*.googleapis.com",
                "https://*.google.com",
                "https://*.gstatic.com",
                "https://*.google-analytics.com"
            };

        var fontSources = new string[] {
                "https://fonts.gstatic.com",
                "https://cdnjs.cloudflare.com"
            };

        var frameSources = new string[] {
                "https://*.google.com",
                "https://challenges.cloudflare.com"
            };

        var imageSources = new string[] {
                "data:",
                "https://*.google.com",
                "https://*.gstatic.com",
                "https://*.googleapis.com",
                "https://*.googleusercontent.com",
                "https://*.google-analytics.com",
                "https://vortex.accuweather.com"
            };

        var reportUris = new string[] {
                "https://mikeandwanus.report-uri.com/r/d/csp/enforce"
            };

        var scriptSources = new string[] {
                // bootstrap
                "https://cdn.jsdelivr.net",
                "https://cdnjs.cloudflare.com",
                "https://stackpath.bootstrapcdn.com",
                "https://*.google.com",
                "https://*.gstatic.com",
                "https://*.googleapis.com",
                "https://*.google-analytics.com",
                "https://*.ggpht.com",
                "https://*.googleusercontent.com",
                "https://www.googletagmanager.com",
                "https://www.accuweather.com",
                "https://oap.accuweather.com",
                "https://vortex.accuweather.com",
                "https://challenges.cloudflare.com"
            };

        var styleSources = new string[] {
                "https://cdnjs.cloudflare.com",
                "https://fonts.googleapis.com",
                "https://vortex.accuweather.com"
            };

        csp
            .DefaultSources(s => s.None())
            .BaseUris(s => s.Self())
            .ConnectSources(s =>
            {
                s.Self();
                s.CustomSources(connectSources);
            })
            .FontSources(s => s.CustomSources(fontSources))
            .FrameSources(s => {
                s.Self();
                s.CustomSources(frameSources);
            })
            .ImageSources(s =>
            {
                s.Self();
                s.CustomSources(imageSources);
            })
            .ManifestSources(s => s.Self())
            .MediaSources(s => s.Self())
            .ObjectSources(s => s.None())
            .ReportUris(s => s.Uris(reportUris))
            .ScriptSources(s =>
            {
                s.Self();
                s.UnsafeInline();
                s.CustomSources(scriptSources);
            })
            .StyleSources(s =>
            {
                s.Self();
                s.UnsafeInline();
                s.CustomSources(styleSources);
            });
    }

    void AddDevPathMappings(IApplicationBuilder app)
    {
        AddDevPathMapping(app, "../client_apps_ng/dist/binary-clock", "/js/binary-clock");
        AddDevPathMapping(app, "../client_apps_ng/dist/googlemaps", "/js/googlemaps");
        AddDevPathMapping(app, "../client_apps_ng/dist/learning", "/js/learning");
        AddDevPathMapping(app, "../client_apps_ng/dist/memory", "/js/memory");
        AddDevPathMapping(app, "../client_apps_ng/dist/money-spin", "/js/money-spin");
        AddDevPathMapping(app, "../client_apps_ng/dist/weekend-countdown", "/js/weekend-countdown");

        AddDevPathMapping(app, "../client_apps/webgl_blender_model/dist", "/js/webgl_blender_model");
        AddDevPathMapping(app, "../client_apps/webgl_cube/dist", "/js/webgl_cube");
        AddDevPathMapping(app, "../client_apps/webgl_shader/dist", "/js/webgl_shader");
        AddDevPathMapping(app, "../client_apps/webgl_text/dist", "/js/webgl_text");
    }

    void AddDevPathMapping(IApplicationBuilder app, string localRelativePath, string urlPath)
    {
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), localRelativePath)),
            RequestPath = new PathString(urlPath),
            ContentTypeProvider = GetCustomMimeTypeProvider()
        });
    }

    FileExtensionContentTypeProvider GetCustomMimeTypeProvider()
    {
        var provider = new FileExtensionContentTypeProvider();

        provider.Mappings[".gltf"] = "model/gltf+json";

        return provider;
    }

    string AddTrailingSlash(string val)
    {
        _ = val ?? throw new ArgumentNullException(nameof(val));

        return val.EndsWith('/') ? val : $"{val}/";
    }
}
