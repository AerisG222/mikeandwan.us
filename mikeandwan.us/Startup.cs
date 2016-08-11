using System;
using System.IO;
using Maw.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
//using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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


namespace MawMvcApp
{
    // TODO: review/fix bootstrap accordian panels (right sidebar), animation is too fast on FF - ok on chrome
    // TODO: research using razor engine for html based emails
    // TODO: nlog?
    // TODO: add 3D demos and eventually picture app
    // TODO: add D3 interface for showing charts on photos/videos apps
    // TODO: create tests for angular apps and .net code
    // TODO: signalr - chat/games
    // TODO: research viewcomponents
    public class Startup
    {
        readonly IConfiguration _config;


        public Startup(IHostingEnvironment hostingEnvironment)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(hostingEnvironment.ContentRootPath)
                .AddJsonFile("config.json")
                .AddUserSecrets();

            MagickWandEnvironment.Genesis();

            _config = builder.Build();
        }


        public void ConfigureServices(IServiceCollection services)
        {
            // http://docs.asp.net/en/latest/fundamentals/configuration.html#options-config-objects
            services
                .Configure<EnvironmentConfig>(_config.GetSection("Environment"))
                .Configure<EmailConfig>(_config.GetSection("Email"))
                .Configure<GoogleCaptchaConfig>(_config.GetSection("Google"))
                .Configure<ContactConfig>(_config.GetSection("ContactUs"));

            services
                .AddDbContext<BlogContext>(options => options.UseNpgsql(_config["Environment:DbConnectionString"]))
                .AddDbContext<IdentityContext>(options => options.UseNpgsql(_config["Environment:DbConnectionString"]))
                .AddDbContext<PhotoContext>(options => options.UseNpgsql(_config["Environment:DbConnectionString"]))
                .AddDbContext<VideoContext>(options => options.UseNpgsql(_config["Environment:DbConnectionString"]));

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

            services.AddAntiforgery(options => 
                {
                    options.HeaderName = "X-XSRF-TOKEN";
                    options.CookieName = "XSRF-TOKEN";
                });

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

            services.AddMvc();
        }


        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IOptions<EnvironmentConfig> envOpts, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            env.EnvironmentName = envOpts.Value.Name;

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/error/");
            }

            /* TODO: see if we need to force any cookie settings
            app.UseCookiePolicy(new CookiePolicyOptions
                {
                    HttpOnly = HttpOnlyPolicy.Always,
                    Secure = CookieSecurePolicy.SameAsRequest
                });
            */

            // TODO: add a build step for production so these urls are published into wwwroot
            if(env.IsDevelopment())
            {
                AddDevPathMappings(app);
            }

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
            AddDevPathMapping(app, "../client_apps/time/dist",              "/js/time");
            AddDevPathMapping(app, "../client_apps/videos/dist",            "/js/videos");
            AddDevPathMapping(app, "../client_apps/webgl_cube/dist",        "/js/webgl_cube");
            AddDevPathMapping(app, "../client_apps/webgl_text/dist",        "/js/webgl_text");
            AddDevPathMapping(app, "../client_apps/weekend_countdown/dist", "/js/weekend_countdown");
        }


        void AddDevPathMapping(IApplicationBuilder app, string localRelativePath, string urlPath)
        {
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), localRelativePath)),
                RequestPath = new PathString(urlPath)
            });
        }
    }
}
