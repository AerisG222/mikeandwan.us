using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.Twitter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using AspNet.Security.OAuth.GitHub;
using Newtonsoft.Json.Linq;
using NMagickWand;
using Maw.Data;
using Maw.Data.Identity;
using Maw.Domain;
using Maw.Domain.Captcha;
using Maw.Domain.Email;
using Maw.Domain.Identity;
using Maw.Domain.Photos;
using MawMvcApp.ViewModels;
using MawMvcApp.ViewModels.About;
using Mvc.RenderViewToString;
using System.IdentityModel.Tokens.Jwt;
using IdentityModel;
using System.Security.Claims;
using System.Linq;
using System.Threading.Tasks;

namespace MawMvcApp
{
    // TODO: googlemaps add async defer back and handle callback when it loads
    // TODO: issue JWT tokens for android app / apis
    // TODO: re-evaluate inline validtion errors once html5 validtion is in place (see: https://github.com/aspnet/Mvc/issues/7035)
    public class Startup
    {
        readonly IConfiguration _config;
        readonly IHostingEnvironment _env;


        public Startup(IConfiguration config, IHostingEnvironment hostingEnvironment)
        {
            _env = hostingEnvironment ?? throw new ArgumentNullException(nameof(hostingEnvironment));
            _config = config ?? throw new ArgumentNullException(nameof(config));

            MagickWandEnvironment.Genesis();
        }


        public void ConfigureServices(IServiceCollection services)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services
                .Configure<ContactConfig>(_config.GetSection("ContactUs"))
                .Configure<GmailApiEmailConfig>(_config.GetSection("Gmail"))
                .Configure<EnvironmentConfig>(_config.GetSection("Environment"))
                .Configure<GoogleCaptchaConfig>(_config.GetSection("GoogleRecaptcha"))
                /*
                .Configure<IdentityOptions>(opts =>
                    {
                        opts.Password.RequiredLength = 8;
                        opts.Password.RequiredUniqueChars = 6;
                    })
                */
                .AddLogging()
                .AddMawDataServices(_config["Environment:DbConnectionString"])
                .AddMawDomainServices()
                .AddTransient<RazorViewToStringRenderer>()
                .AddSingleton<IFileProvider>(x => new PhysicalFileProvider(_config["Environment:AssetsPath"]))
                .AddAntiforgery(opts => opts.HeaderName = "X-XSRF-TOKEN")

                /*
                .AddIdentity<MawUser, MawRole>()
                    .AddDefaultTokenProviders()
                    .Services
                .ConfigureApplicationCookie(opts => {
                    opts.AccessDeniedPath = "/account/access-denied";
                    opts.Cookie.Name = "maw_auth";
                    opts.ExpireTimeSpan = TimeSpan.FromMinutes(15);
                    opts.LoginPath = "/account/login";
                    opts.LogoutPath = "/account/logout";

                    if(_env.IsStaging())
                    {
                        opts.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                    }
                })
                */

                .AddAuthentication(opts => {
                    opts.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    opts.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, opts => {
                    opts.AccessDeniedPath = "/account/access-denied";
                })
                .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, opts => {
                    opts.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    opts.Authority = "http://localhost:5010";
                    opts.RequireHttpsMetadata = false;

                    opts.ClientId = "www.mikeandwan.us";
                    opts.ClientSecret = "secret";
                    opts.ResponseType = "code id_token";

                    opts.SaveTokens = true;
                    opts.GetClaimsFromUserInfoEndpoint = true;

                    // core
                    opts.Scope.Add("openid");
                    opts.Scope.Add("profile");
                    opts.Scope.Add("offline_access");

                    // apis
                    opts.Scope.Add("admin");
                    opts.Scope.Add("blog");
                    opts.Scope.Add("photo");
                    opts.Scope.Add("video");

                    // identity resources
                    opts.Scope.Add("roles");
                    opts.Scope.Add("email");

                    opts.ClaimActions.Add(new RoleClaimAction());

                    //opts.ClaimActions.MapUniqueJsonKey("family_name", "family_name");

                    /* alternative to RoleClaimAction above
                    // https://stackoverflow.com/questions/46038509/unable-to-retrieve-claims-in-net-core-2-0
                    opts.Events = new OpenIdConnectEvents()
                    {
                        OnUserInformationReceived = (context) =>
                        {
                            ClaimsIdentity claimsId = context.Principal.Identity as ClaimsIdentity;

                            var roles = context.User.Children().FirstOrDefault(j => j.Path == JwtClaimTypes.Role).Values().ToList();
                            claimsId.AddClaims(roles.Select(r => new Claim(JwtClaimTypes.Role, r.Value<String>())));

                            return Task.FromResult(0);
                        }
                    };
                    */

                    opts.TokenValidationParameters.NameClaimType = JwtClaimTypes.Name;
                    opts.TokenValidationParameters.RoleClaimType = JwtClaimTypes.Role;
                })

                /*
                .AddGitHub(opts =>
                    {
                        opts.ClientId = _config["GitHub:ClientId"];
                        opts.ClientSecret = _config["GitHub:ClientSecret"];
                        opts.SaveTokens = true;
                        opts.Scope.Add("user:email");
                    })
                .AddGoogle(opts =>
                    {
                        opts.ClientId = _config["GooglePlus:ClientId"];
                        opts.ClientSecret = _config["GooglePlus:ClientSecret"];
                        opts.SaveTokens = true;
                    })
                .AddMicrosoftAccount(opts =>
                    {
                        opts.ClientId = _config["Microsoft:ApplicationId"];
                        opts.ClientSecret = _config["Microsoft:Secret"];
                        opts.SaveTokens = true;
                    })
                .AddTwitter(opts =>
                    {
                        opts.ConsumerKey = _config["Twitter:ConsumerKey"];
                        opts.ConsumerSecret = _config["Twitter:ConsumerSecret"];
                        opts.RetrieveUserDetails = true;
                        opts.SaveTokens = true;
                    })
                */
                .Services

                .AddAuthorization(opts =>
                    {
                        opts.AddPolicy(MawConstants.POLICY_VIEW_PHOTOS, new AuthorizationPolicyBuilder().RequireRole(MawConstants.ROLE_FRIEND, MawConstants.ROLE_ADMIN).Build());
                        opts.AddPolicy(MawConstants.POLICY_VIEW_VIDEOS, new AuthorizationPolicyBuilder().RequireRole(MawConstants.ROLE_FRIEND, MawConstants.ROLE_ADMIN).Build());
                        opts.AddPolicy(MawConstants.POLICY_ADMIN_SITE, new AuthorizationPolicyBuilder().RequireRole(MawConstants.ROLE_ADMIN).Build());
                    })
                .AddMvc();

                if(_env.IsDevelopment())
                {
                    services.AddMiniProfiler();
                }
        }


        public void Configure(IApplicationBuilder app)
        {
            if (_env.IsDevelopment())
            {
                app.UseMiniProfiler();
                app.UseDeveloperExceptionPage();
                AddDevPathMappings(app);
            }
            else
            {
                app.UseExceptionHandler("/error/");
            }

            app
                .UseForwardedHeaders(new ForwardedHeadersOptions
                    {
                        ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
                    })
                /*
                .UseCookiePolicy(new CookiePolicyOptions
                    {
                        // note: using the forwarded middleware allowed the auth ticket to be marked secure.  however, the antiforgery
                        //       only seems to be set when the antiforgery option is configured with RequireSsl = true.  The policy
                        //       below ensures that all cookies are forced to match the request
                        //       [HttpOnly will interfere with custom XSRF API filters]

                        //HttpOnly = HttpOnlyPolicy.Always,
                        Secure = CookieSecurePolicy.SameAsRequest
                    })
                */
                .UseAuthentication()
                .UseStaticFiles()
                .UseMvc();
        }


        void AddDevPathMappings(IApplicationBuilder app)
        {
            AddDevPathMapping(app, "../client_apps/bandwidth/dist",         "/js/bandwidth");
            AddDevPathMapping(app, "../client_apps/binary_clock/dist",      "/js/binary_clock");
            AddDevPathMapping(app, "../client_apps/byte_counter/dist",      "/js/byte_counter");
            AddDevPathMapping(app, "../client_apps/filesize/dist",          "/js/filesize");
            AddDevPathMapping(app, "../client_apps/googlemaps/dist",        "/js/googlemaps");
            AddDevPathMapping(app, "../client_apps/learning/dist",          "/js/learning");
            AddDevPathMapping(app, "../client_apps/memory/dist",            "/js/memory");
            AddDevPathMapping(app, "../client_apps/money_spin/dist",        "/js/money_spin");
            AddDevPathMapping(app, "../client_apps/photos/dist",            "/js/photos");
            AddDevPathMapping(app, "../client_apps/photos3d/dist",          "/js/photos3d");
            AddDevPathMapping(app, "../client_apps/photo_stats/dist",       "/js/photo_stats");
            AddDevPathMapping(app, "../client_apps/time/dist",              "/js/time");
            AddDevPathMapping(app, "../client_apps/videos/dist",            "/js/videos");
            AddDevPathMapping(app, "../client_apps/webgl_cube/dist",        "/js/webgl_cube");
            AddDevPathMapping(app, "../client_apps/webgl_text/dist",        "/js/webgl_text");
            AddDevPathMapping(app, "../client_apps/weekend_countdown/dist", "/js/weekend_countdown");
        }


        void AddDevPathMapping(IApplicationBuilder app, string localRelativePath, string urlPath)
        {
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), localRelativePath)),
                RequestPath = new PathString(urlPath)
            });
        }
    }
}
