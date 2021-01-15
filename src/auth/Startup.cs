using System;
using System.IO;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Duende.IdentityServer;
using Mvc.RenderViewToString;
using NWebsec.Core.Common.Middleware.Options;
using Maw.Data;
using Maw.Domain;
using Maw.Domain.Email;
using Maw.Domain.Identity;
using Maw.Security;
using Maw.TagHelpers;
using MawAuth.Models;
using MawAuth.Services;


namespace MawAuth
{
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
                _config["Environment:WwwUrl"],
                _config["Environment:WwwClientSecret"],
                _config["Environment:PhotosUrl"],
                _config["Environment:FilesUrl"]);

            ConfigureDataProtection(services);

            services
                .Configure<IdentityOptions>(opts =>
                    {
                        opts.Password.RequiredLength = 8;
                        opts.Password.RequiredUniqueChars = 6;
                    })
                .Configure<GmailApiEmailConfig>(_config.GetSection("Gmail"))
                .ConfigureMawTagHelpers(opts => {
                    opts.AuthUrl = AddTrailingSlash(_config["Environment:AuthUrl"]);
                    opts.WwwUrl = AddTrailingSlash(_config["Environment:WwwUrl"]);
                })
                .AddMawDataServices(_config["Environment:DbConnectionString"])
                .AddMawDomainServices()
                .AddMawIdentityServerServices(_config["Environment:IdsrvDbConnectionString"], _config["SigningCertDir"])
                .AddTransient<RazorViewToStringRenderer>()
                .AddIdentity<MawUser, MawRole>()
                    .AddDefaultTokenProviders()
                    .Services
                .ConfigureApplicationCookie(opts => {
                    opts.AccessDeniedPath = "/account/access-denied";
                    opts.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                    opts.LoginPath = "/account/login";
                    opts.LogoutPath = "/account/logout";
                })
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddGitHub(opts =>
                    {
                        opts.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                        opts.ClientId = _config["GitHub:ClientId"];
                        opts.ClientSecret = _config["GitHub:ClientSecret"];
                        opts.Scope.Add("user:email");
                    })
                .AddGoogle(opts =>
                    {
                        // https://github.com/aspnet/AspNetCore/issues/6486
                        opts.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                        opts.ClientId = _config["GooglePlus:ClientId"];
                        opts.ClientSecret = _config["GooglePlus:ClientSecret"];
                        opts.UserInformationEndpoint = "https://www.googleapis.com/oauth2/v2/userinfo";
                        opts.ClaimActions.Clear();
                        opts.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
                        opts.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
                        opts.ClaimActions.MapJsonKey(ClaimTypes.GivenName, "given_name");
                        opts.ClaimActions.MapJsonKey(ClaimTypes.Surname, "family_name");
                        opts.ClaimActions.MapJsonKey("urn:google:profile", "link");
                        opts.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
                    })
                .AddMicrosoftAccount(opts =>
                    {
                        opts.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                        opts.ClientId = _config["Microsoft:ApplicationId"];
                        opts.ClientSecret = _config["Microsoft:Secret"];
                    })
                .AddTwitter(opts =>
                    {
                        opts.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                        opts.ConsumerKey = _config["Twitter:ConsumerKey"];
                        opts.ConsumerSecret = _config["Twitter:ConsumerSecret"];
                        opts.RetrieveUserDetails = true;
                    })
                    .Services
                .AddIdentityServer(opts =>
                    {
                        // we need to set this especially for dev otherwise the issuer becomes 10.0.2.2 when testing the android app
                        opts.IssuerUri = _config["Environment:AuthUrl"];
                    })
                    .AddMawIdentityServerKeyMaterial(_config["SigningCertDir"])
                    .AddInMemoryApiScopes(config.GetApiScopes())
                    .AddInMemoryApiResources(config.GetApiResources())
                    .AddInMemoryClients(config.GetClients())
                    .AddInMemoryIdentityResources(config.GetIdentityResources())
                    .AddAspNetIdentity<MawUser>()
                    .AddProfileService<IdentityServerProfileService>()
                    .Services
                .AddAuthorization(opts =>
                    {
                        MawPolicyBuilder.AddMawPolicies(opts);
                    })
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

        string[] GetAllowedRedirectUrls()
        {
            return new string[] {
                AddTrailingSlash(_config["Environment:FilesUrl"]),
                AddTrailingSlash(_config["Environment:PhotosUrl"]),
                AddTrailingSlash(_config["Environment:WwwUrl"]),
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

            var reportUris = new string[] {
                "https://mikeandwanus.report-uri.com/r/d/csp/enforce"
            };

            var scriptSources = new string[] {
                "https://code.jquery.com",
                "https://cdn.jsdelivr.net",
                "https://stackpath.bootstrapcdn.com"
            };

            var styleSources = new string[] {
                "https://fonts.googleapis.com"
            };

            csp
                .DefaultSources(s => s.None())
                .FontSources(s => s.CustomSources(fontSources))
                .ImageSources(s => s.Self())
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
            if(val == null) {
                return val;
            }

            return val.EndsWith('/') ? val : $"{val}/";
        }
    }
}
