using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Maw.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Maw.Domain.Identity;
using Maw.Domain;
using MawAuth.Services;
using IdentityServer4;
using Microsoft.AspNetCore.Mvc;
using Mvc.RenderViewToString;
using Maw.Security;
using MawAuth.Models;


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
            services
                .Configure<IdentityOptions>(opts =>
                    {
                        opts.Password.RequiredLength = 8;
                        opts.Password.RequiredUniqueChars = 6;
                    })
                .AddMawDataServices(_config["Environment:DbConnectionString"])
                .AddMawDomainServices()
                .AddTransient<RazorViewToStringRenderer>()
                .AddIdentity<MawUser, MawRole>()
                    .AddDefaultTokenProviders()
                    .Services
                .ConfigureApplicationCookie(opts => {
                    opts.AccessDeniedPath = "/account/access-denied";
                    opts.ExpireTimeSpan = TimeSpan.FromMinutes(15);
                    opts.LoginPath = "/account/login";
                    opts.LogoutPath = "/account/logout";

                    if(_env.IsStaging())
                    {
                        opts.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                    }
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
                        opts.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                        opts.ClientId = _config["GooglePlus:ClientId"];
                        opts.ClientSecret = _config["GooglePlus:ClientSecret"];
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
                    });

            var config = new Config(_config["Environment:WwwUrl"], _config["Environment:WwwClientSecret"]);

            var idsrv = services
                .AddIdentityServer(opts =>
                    {
                        // we need to set this especially for dev otherwise the issuer becomes 10.0.2.2 when testing the android app
                        opts.IssuerUri = _config["Environment:AuthUrl"];
                    })
                    .AddInMemoryPersistedGrants()
                    .AddInMemoryApiResources(config.GetApiResources())
                    .AddInMemoryClients(config.GetClients())
                    .AddInMemoryIdentityResources(config.GetIdentityResources())
                    .AddAspNetIdentity<MawUser>()
                    .AddProfileService<IdentityServerProfileService>();

            if (_env.IsDevelopment())
            {
                idsrv.AddDeveloperSigningCredential();
            }

            services
                .AddAuthorization(opts =>
                    {
                        MawPolicyBuilder.AddMawPolicies(opts);
                    })
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
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
