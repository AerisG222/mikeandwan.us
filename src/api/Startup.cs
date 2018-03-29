﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel;
using Maw.Data;
using Maw.Domain;
using Maw.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;


namespace MawApi
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
            var authConfig = new AuthConfig();
            var corsConfig = new CorsConfig();

            _config.GetSection("AuthConfig").Bind(authConfig);
            _config.GetSection("CorsConfig").Bind(corsConfig);

            services
                .Configure<EnvironmentConfig>(_config.GetSection("Environment"))
                .AddMawDataServices(_config["Environment:DbConnectionString"])
                .AddMawDomainServices()
                .AddMvcCore()
                    .AddAuthorization()
                    .AddJsonFormatters()
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                    .Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddIdentityServerAuthentication(opts => {
                        opts.Authority = authConfig.AuthorizationUrl;

                        opts.ApiName = "maw_api";

                        opts.NameClaimType = JwtClaimTypes.Name;
                        opts.RoleClaimType = JwtClaimTypes.Role;
                    })
                    .Services
                .AddAuthorization(opts => {
                        MawPolicyBuilder.AddMawPolicies(opts);
                    })
                .AddCors(opts => {
                    // this defines a CORS policy called "default"
                    opts.AddPolicy("default", policy => {
                        policy.WithOrigins(corsConfig.SiteUrl)
                            .AllowCredentials()
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
                });
        }


        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseCors("default");
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
