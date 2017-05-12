using System;
using System.IO;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
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
//using AspNet.Security.OAuth.GitHub;
using NLog.Web;
using NMagickWand;
using Maw.Data;
using Maw.Data.Identity;
using Maw.Domain;
using Maw.Domain.Captcha;
using Maw.Domain.Email;
using Maw.Domain.Identity;
using MawMvcApp.ViewModels;
using MawMvcApp.ViewModels.About;


namespace MawMvcApp
{
    // TODO: googlemaps add async defer back and handle callback when it loads
    // TODO: add github auth again
    public class Startup
    {
        readonly IConfiguration _config;
        readonly IHostingEnvironment _env;


        public Startup(IHostingEnvironment hostingEnvironment)
        {
            _env = hostingEnvironment;

            var builder = new ConfigurationBuilder()
                .SetBasePath(hostingEnvironment.ContentRootPath)
                .AddJsonFile("config.json")
                .AddEnvironmentVariables("MAW_");

            MagickWandEnvironment.Genesis();

            _config = builder.Build();
        }


        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddSingleton<FrameworkIdentifier>()
                .Configure<ContactConfig>(_config.GetSection("ContactUs"))
                .Configure<EmailConfig>(_config.GetSection("Email"))
                .Configure<EnvironmentConfig>(_config.GetSection("Environment"))
                .Configure<GoogleCaptchaConfig>(_config.GetSection("GoogleRecaptcha"))
                .Configure<IdentityOptions>(opts =>
                    {
                        // relax the pwd requirements to match previous settings
                        // future enhancement would be to enable and force users to change, but this
                        // is additionally complicated as the logic should be replicated in both this
                        // and the android app.
                        opts.Password.RequireDigit = false;
                        opts.Password.RequireLowercase = false;
                        opts.Password.RequireNonAlphanumeric = false;
                        opts.Password.RequireUppercase = false;
                        opts.Password.RequiredLength = 4;

                        opts.Cookies.ApplicationCookie.CookieName = "maw_auth";
                        opts.Cookies.ApplicationCookie.LoginPath = "/account/login";
                        opts.Cookies.ApplicationCookie.LogoutPath = "/account/logout";
                        opts.Cookies.ApplicationCookie.AccessDeniedPath = "/account/access-denied";
                        opts.Cookies.ApplicationCookie.ExpireTimeSpan = new TimeSpan(0, 20, 0);

                        opts.Cookies.ExternalCookie.CookieName = "ext_auth";
                        opts.Cookies.ExternalCookie.LoginPath = "/account/login";
                        opts.Cookies.ExternalCookie.LogoutPath = "/account/logout";
                        opts.Cookies.ExternalCookie.AccessDeniedPath = "/account/access-denied";
                        opts.Cookies.ExternalCookie.ExpireTimeSpan = new TimeSpan(0, 20, 0);
                    })
                .AddMawDataRepositories(_config["Environment:DbConnectionString"])
                .AddScoped<IFileProvider>(x => new PhysicalFileProvider(_config["Environment:AssetsPath"]))
                .AddScoped<ICaptchaService, GoogleCaptchaService>()
                .AddScoped<IEmailService, EmailService>()
                .AddScoped<ILoginService, LoginService>()
                .AddScoped<IUserStore<MawUser>, MawUserStore>()
                .AddScoped<IRoleStore<MawRole>, MawRoleStore>()
                .AddAntiforgery(opts => opts.HeaderName = "X-XSRF-TOKEN")
                .AddLogging()
                .AddIdentity<MawUser, MawRole>()
                    .AddDefaultTokenProviders()
                    .Services
                .AddGoogleAuthentication(opts => 
                    {
                        opts.ClientId = _config["GooglePlus:ClientId"];
                        opts.ClientSecret = _config["GooglePlus:ClientSecret"];
                    })
                .AddMicrosoftAccountAuthentication(opts =>
                    {
                        opts.ClientId = _config["Microsoft:ApplicationId"];
                        opts.ClientSecret = _config["Microsoft:Secret"];
                    })
                .AddTwitterAuthentication(opts =>
                    {
                        opts.ConsumerKey = _config["Twitter:ConsumerKey"];
                        opts.ConsumerSecret = _config["Twitter:ConsumerSecret"];
                        opts.RetrieveUserDetails = true;
                    })
                .AddAuthorization(opts =>
                    {
                        opts.AddPolicy(MawConstants.POLICY_VIEW_PHOTOS, new AuthorizationPolicyBuilder().RequireRole(MawConstants.ROLE_FRIEND, MawConstants.ROLE_ADMIN).Build());
                        opts.AddPolicy(MawConstants.POLICY_VIEW_VIDEOS, new AuthorizationPolicyBuilder().RequireRole(MawConstants.ROLE_FRIEND, MawConstants.ROLE_ADMIN).Build());
                        opts.AddPolicy(MawConstants.POLICY_ADMIN_SITE, new AuthorizationPolicyBuilder().RequireRole(MawConstants.ROLE_ADMIN).Build());
                    })
                .AddMvc();
        }


        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            if (_env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                AddDevPathMappings(app);
            }
            else
            {
                app.AddNLogWeb();
                _env.ConfigureNLog("nlog.config");

                app.UseExceptionHandler("/error/");
            }

            app
                .UseForwardedHeaders(new ForwardedHeadersOptions
                    {
                        ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
                    })
                .UseCookiePolicy(new CookiePolicyOptions
                    {
                        // note: using the forwarded middleware allowed the auth ticket to be marked secure.  however, the antiforgery
                        //       only seems to be set when the antiforgery option is configured with RequireSsl = true.  The policy
                        //       below ensures that all cookies are forced to match the request
                        //       [HttpOnly will interfere with custom XSRF API filters]

                        //HttpOnly = HttpOnlyPolicy.Always,
                        Secure = CookieSecurePolicy.SameAsRequest
                    })
                .UseAuthentication()
                //.UseCookieAuthentication()
                /*
                .UseGitHubAuthentication(new GitHubAuthenticationOptions
                    {
                        ClientId = _config["GitHub:ClientId"],
                        ClientSecret = _config["GitHub:ClientSecret"],
                        Scope = { "user:email" }
                    })
                */
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
