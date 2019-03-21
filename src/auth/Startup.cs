using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using IdentityServer4;
using IdentityServer4.Stores;
using Mvc.RenderViewToString;
using Maw.Data;
using Maw.Domain;
using Maw.Domain.Email;
using Maw.Domain.Identity;
using Maw.Security;
using MawAuth.Models;
using MawAuth.Services;


namespace MawAuth
{
    public class Startup
    {
        readonly IConfiguration _config;
        readonly IHostingEnvironment _env;


        public Startup(IConfiguration config, IHostingEnvironment hostingEnvironment)
        {
            _env = hostingEnvironment ?? throw new ArgumentNullException(nameof(hostingEnvironment));
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }


        public void ConfigureServices(IServiceCollection services)
        {
            var config = new Config(
                _config["Environment:WwwUrl"],
                _config["Environment:WwwClientSecret"],
                _config["Environment:PhotosUrl"],
                _config["Environment:FilesUrl"]);

            services
                .Configure<IdentityOptions>(opts =>
                    {
                        opts.Password.RequiredLength = 8;
                        opts.Password.RequiredUniqueChars = 6;
                    })
                .Configure<GmailApiEmailConfig>(_config.GetSection("Gmail"))
                .AddMawDataServices(_config["Environment:DbConnectionString"])
                .AddMawDomainServices()
                .AddMawIdentityServerServices(_config["Environment:IdsrvDbConnectionString"], _config["SigningCertDir"])
                .AddTransient<RazorViewToStringRenderer>()
                .AddIdentity<MawUser, MawRole>()
                    .AddDefaultTokenProviders()
                    .Services
                .ConfigureApplicationCookie(opts => {
                    opts.AccessDeniedPath = "/account/access-denied";
                    opts.ExpireTimeSpan = TimeSpan.FromMinutes(15);
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
                /*
                .AddGoogle(opts =>
                    {
                        opts.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                        opts.ClientId = _config["GooglePlus:ClientId"];
                        opts.ClientSecret = _config["GooglePlus:ClientSecret"];
                    })
                */
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
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }


        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error/");
            }

            app.UseStaticFiles();
            app.UseIdentityServer();
            app.UseMvc();
        }
    }
}
