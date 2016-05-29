using System;
using Maw.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
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
        IConfiguration _config;


        public Startup(IHostingEnvironment hostingEnvironment)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(hostingEnvironment.ContentRootPath)
                .AddJsonFile("config.json")
                .AddUserSecrets();

            MagickWandEnvironment.Genesis();
            
            _config = builder.Build();
        }

        
        public IServiceProvider ConfigureServices(IServiceCollection services)
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
                .AddScoped<IBlogRepository, BlogRepository>()
                .AddScoped<IUserRepository, UserRepository>()
                .AddScoped<IPhotoRepository, PhotoRepository>()
                .AddScoped<IVideoRepository, VideoRepository>()
                .AddScoped<IUserStore<MawUser>, MawUserStore>()
                .AddScoped<IRoleStore<MawRole>, MawRoleStore>();
            
            // because nginx is terminating the ssl connection, we must check the environment to determine if we should use 
            // secure cookies or not, but let's default to the request before we override for prod
            var secureCookieOption = CookieSecureOption.SameAsRequest;
            
            if(string.Equals("production", _config["Environment:Name"], StringComparison.OrdinalIgnoreCase))
            {
                secureCookieOption = CookieSecureOption.Always;
            }
            
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
                    opts.Cookies.ApplicationCookie.CookieSecure = secureCookieOption;
                    opts.Cookies.ApplicationCookie.LoginPath = "/account/login";
                    opts.Cookies.ApplicationCookie.LogoutPath = "/account/logout";
                    opts.Cookies.ApplicationCookie.AccessDeniedPath = "/account/access-denied";
                    opts.Cookies.ApplicationCookie.ExpireTimeSpan = new TimeSpan(0, 20, 0);  // auth timeout = 20min [sliding]
                })
                .AddDefaultTokenProviders();

            // TODO: enable secure flag on antiforgery cookies
            //services.ConfigureAntiforgery(options =>
            //    {
            //        options.RequireSsl = true;
            //    });
                
            services.AddAuthorization(opts =>
                {
                    opts.AddPolicy(MawConstants.POLICY_VIEW_PHOTOS, new AuthorizationPolicyBuilder().RequireRole(MawConstants.ROLE_FRIEND, MawConstants.ROLE_ADMIN).Build());
                    opts.AddPolicy(MawConstants.POLICY_VIEW_VIDEOS, new AuthorizationPolicyBuilder().RequireRole(MawConstants.ROLE_FRIEND, MawConstants.ROLE_ADMIN).Build());
                    opts.AddPolicy(MawConstants.POLICY_ADMIN_SITE, new AuthorizationPolicyBuilder().RequireRole(MawConstants.ROLE_ADMIN).Build());
                });
            
            var mvc = services.AddMvc()
                .AddJsonOptions(opts =>
                    {
                        opts.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    });
            
            /*
            if(string.Equals("production", _config["Environment:Name"], StringComparison.OrdinalIgnoreCase))
            {
                mvc.AddPrecompiledRazorViews(GetType().Assembly);
            }
            */
            
            return services.BuildServiceProvider();
        }


        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IOptions<EnvironmentConfig> envOpts, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();
            
            env.EnvironmentName = envOpts.Value.Name;
            
            if (envOpts.Value.IsDevelopment)
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/error/");
            }

            app.UseIdentity();
            app.UseStaticFiles();
            app.UseMvc();
        }
    }
}
