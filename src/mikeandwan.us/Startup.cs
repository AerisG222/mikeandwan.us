using System;
using System.IO;
using Maw.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using NLog.Web;
using NMagickWand;
using Maw.Data.EntityFramework.Blogs;
using Maw.Data.EntityFramework.Identity;
using Maw.Data.EntityFramework.Photos;
using Maw.Data.EntityFramework.Videos;
using Maw.Data.Identity;
using Maw.Domain;
using Maw.Domain.Blogs;
using Maw.Domain.Captcha;
using Maw.Domain.Email;
using Maw.Domain.Identity;
using Maw.Domain.Videos;
using Maw.Domain.Photos;
using MawMvcApp.ViewModels;
using MawMvcApp.ViewModels.About;
using Microsoft.AspNetCore.Mvc;

namespace MawMvcApp
{
    // TODO: googlemaps add async defer back and handle callback when it loads
    // TODO: upgrade EF to 1.1.2 once it comes out, which should fix https://github.com/aspnet/EntityFramework/issues/7811
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
            services.AddSingleton<FrameworkIdentifier>();

            // http://docs.asp.net/en/latest/fundamentals/configuration.html#options-config-objects
            services
                .Configure<EnvironmentConfig>(_config.GetSection("Environment"))
                .Configure<EmailConfig>(_config.GetSection("Email"))
                .Configure<GoogleCaptchaConfig>(_config.GetSection("Google"))
                .Configure<ContactConfig>(_config.GetSection("ContactUs"));

            services
                .AddDbContext<BlogContext>(opts => opts.UseNpgsql(_config["Environment:DbConnectionString"]))
                .AddDbContext<IdentityContext>(opts => opts.UseNpgsql(_config["Environment:DbConnectionString"]))
                .AddDbContext<PhotoContext>(opts => opts.UseNpgsql(_config["Environment:DbConnectionString"]))
                .AddDbContext<VideoContext>(opts => opts.UseNpgsql(_config["Environment:DbConnectionString"]));

            services
                .AddScoped<IFileProvider>(x => new PhysicalFileProvider(_config["Environment:AssetsPath"]))
                .AddScoped<ICaptchaService, GoogleCaptchaService>()
                .AddScoped<IEmailService, EmailService>()
                .AddScoped<ILoginService, LoginService>()
                .AddScoped<IBlogRepository, BlogRepository>()
                .AddScoped<IUserRepository, UserRepository>()
                .AddScoped<IPhotoRepository, PhotoRepository>()
                .AddScoped<IVideoRepository, VideoRepository>()
                .AddScoped<IUserStore<MawUser>, MawUserStore>()
                .AddScoped<IRoleStore<MawRole>, MawRoleStore>();

            services.AddAntiforgery(opts => opts.HeaderName = "X-XSRF-TOKEN");

            services.AddLogging();

            services.AddIdentity<MawUser, MawRole>(opts =>
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
                    opts.Cookies.ApplicationCookie.ExpireTimeSpan = new TimeSpan(0, 20, 0);  // auth timeout = 20min [sliding]
                })
                .AddDefaultTokenProviders();

            services.AddAuthorization(opts =>
                {
                    opts.AddPolicy(MawConstants.POLICY_VIEW_PHOTOS, new AuthorizationPolicyBuilder().RequireRole(MawConstants.ROLE_FRIEND, MawConstants.ROLE_ADMIN).Build());
                    opts.AddPolicy(MawConstants.POLICY_VIEW_VIDEOS, new AuthorizationPolicyBuilder().RequireRole(MawConstants.ROLE_FRIEND, MawConstants.ROLE_ADMIN).Build());
                    opts.AddPolicy(MawConstants.POLICY_ADMIN_SITE, new AuthorizationPolicyBuilder().RequireRole(MawConstants.ROLE_ADMIN).Build());
                });

            services.AddMvc(opts => 
                {
                    if(_env.IsProduction())
                    {
                        // this should already be set in nginx, but this protects us if that is misconfigured
                        opts.Filters.Add(new RequireHttpsAttribute());
                    }
                });
        }


        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            if (_env.IsDevelopment())
            {
                loggerFactory.AddConsole();

                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();

                AddDevPathMappings(app);
            }
            else
            {
                app.AddNLogWeb();
                _env.ConfigureNLog("nlog.config");

                app.UseExceptionHandler("/error/");
            }

            app.UseForwardedHeaders(new ForwardedHeadersOptions
                {
                    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
                });

            // note: using the forwarded middleware allowed the auth ticket to be marked secure.  however, the antiforgery
            //       only seems to be set when the antiforgery option is configured with RequireSsl = true.  The policy
            //       below ensures that all cookies are forced to match the request
            //       [HttpOnly will interfere with custom XSRF API filters]
            app.UseCookiePolicy(new CookiePolicyOptions
                {
                    //HttpOnly = HttpOnlyPolicy.Always,
                    Secure = CookieSecurePolicy.SameAsRequest
                });

            app.UseIdentity();
            app.UseStaticFiles();
            app.UseMvc();
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
