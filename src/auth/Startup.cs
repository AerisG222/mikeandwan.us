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
                .AddIdentity<MawUser, MawRole>()
                    .AddDefaultTokenProviders()
                    .Services
                .ConfigureApplicationCookie(opts => {
                    opts.AccessDeniedPath = "/account/access-denied";
                    //opts.Cookie.Name = "maw_auth";
                    opts.ExpireTimeSpan = TimeSpan.FromMinutes(15);
                    opts.LoginPath = "/account/login";
                    opts.LogoutPath = "/account/logout";

                    if(_env.IsStaging())
                    {
                        opts.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                    }
                })
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme);

            var idsrv = services
                .AddIdentityServer()
                    .AddInMemoryPersistedGrants()
                    .AddInMemoryApiResources(Config.GetApiResources())
                    .AddInMemoryClients(Config.GetClients())
                    .AddInMemoryIdentityResources(Config.GetIdentityResources())
                    .AddAspNetIdentity<MawUser>();

            if (_env.IsDevelopment())
            {
                idsrv.AddDeveloperSigningCredential();
            }

            services.AddMvc();
        }


        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseIdentityServer();
            app.UseMvc();
        }
    }
}
