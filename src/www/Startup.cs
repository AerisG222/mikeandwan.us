using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using IdentityModel;
using Mvc.RenderViewToString;
using NMagickWand;
using Maw.Data;
using Maw.Domain;
using Maw.Domain.Captcha;
using Maw.Domain.Email;
using Maw.Security;
using MawMvcApp.ViewModels.About;


namespace MawMvcApp
{
    // TODO: googlemaps add async defer back and handle callback when it loads
    // TODO: re-evaluate inline validation errors once html5 validation is in place (see: https://github.com/aspnet/Mvc/issues/7035)
    public class Startup
    {
        readonly IConfiguration _config;
        readonly IWebHostEnvironment _env;


        public Startup(IConfiguration config, IWebHostEnvironment hostingEnvironment)
        {
            _env = hostingEnvironment ?? throw new ArgumentNullException(nameof(hostingEnvironment));
            _config = config ?? throw new ArgumentNullException(nameof(config));

            MagickWandEnvironment.Genesis();
        }


        public void ConfigureServices(IServiceCollection services)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            var authConfig = new AuthConfig();
            _config.GetSection("AuthConfig").Bind(authConfig);

            services
                .Configure<ContactConfig>(_config.GetSection("ContactUs"))
                .Configure<GmailApiEmailConfig>(_config.GetSection("Gmail"))
                .Configure<EnvironmentConfig>(_config.GetSection("Environment"))
                .Configure<GoogleCaptchaConfig>(_config.GetSection("GoogleRecaptcha"))
                .AddLogging()
                .AddMawDataServices(_config["Environment:DbConnectionString"])
                .AddMawDomainServices()
                .AddTransient<RazorViewToStringRenderer>()
                .AddScoped<IContentTypeProvider, FileExtensionContentTypeProvider>()
                .AddSingleton<IFileProvider>(x => new PhysicalFileProvider(_config["Environment:AssetsPath"]))
                .AddAntiforgery(opts => opts.HeaderName = "X-XSRF-TOKEN")
                .AddAuthentication(opts => {
                    opts.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    opts.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, opts => {
                    opts.AccessDeniedPath = "/account/access-denied";
                    opts.LogoutPath = "/account/logout";
                    opts.ExpireTimeSpan = TimeSpan.FromMinutes(15);
                })
                .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, opts => {
                    opts.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    opts.Authority = authConfig.AuthorizationUrl;

                    opts.ClientId = authConfig.ClientId;
                    opts.ClientSecret = authConfig.Secret;
                    opts.ResponseType = "code id_token";

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
                })
                .Services
                .AddAuthorization(opts =>
                    {
                        MawPolicyBuilder.AddMawPolicies(opts);
                    })
                .AddCors(opts => {
                    opts.AddPolicy("default", policy => {
                        policy
                            .AllowAnyOrigin()
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
                })
                .AddControllersWithViews();

                if(_env.IsDevelopment())
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
                app.UseExceptionHandler("/error/");
            }

            app
                .UseStaticFiles(new StaticFileOptions {
                    ContentTypeProvider = GetCustomMimeTypeProvider()
                })
                .UseRouting()
                .UseCors("default")
                .UseForwardedHeaders(new ForwardedHeadersOptions
                    {
                        ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
                    })
                .UseAuthentication()
                .UseAuthorization()
                .UseEndpoints(endpoints => {
                    endpoints.MapControllers();
                });
        }


        void AddDevPathMappings(IApplicationBuilder app)
        {
            AddDevPathMapping(app, "../client_apps/binary_clock/dist",        "/js/binary_clock");
            AddDevPathMapping(app, "../client_apps/googlemaps/dist",          "/js/googlemaps");
            AddDevPathMapping(app, "../client_apps/learning/dist",            "/js/learning");
            AddDevPathMapping(app, "../client_apps/memory/dist",              "/js/memory");
            AddDevPathMapping(app, "../client_apps/money_spin/dist",          "/js/money_spin");
            AddDevPathMapping(app, "../client_apps/webgl_blender_model/dist", "/js/webgl_blender_model");
            AddDevPathMapping(app, "../client_apps/webgl_cube/dist",          "/js/webgl_cube");
            AddDevPathMapping(app, "../client_apps/webgl_shader/dist",        "/js/webgl_shader");
            AddDevPathMapping(app, "../client_apps/webgl_text/dist",          "/js/webgl_text");
            AddDevPathMapping(app, "../client_apps/weekend_countdown/dist",   "/js/weekend_countdown");
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
    }
}
