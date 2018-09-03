using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using IdentityModel;
using Maw.Data;
using Maw.Domain;
using Maw.Domain.Upload;
using Maw.Security;
using MawApi.Hubs;


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
                .Configure<UploadConfig>(_config.GetSection("FileUpload"))
                .AddMawDataServices(_config["Environment:DbConnectionString"])
                .AddMawDomainServices()
                .AddScoped<IContentTypeProvider, FileExtensionContentTypeProvider>()
                .AddMvcCore()
                    .AddAuthorization()
                    .AddJsonFormatters()
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                    .Services
                .AddSignalR()
                    .AddMessagePackProtocol()
                    .Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(opts => {
                        opts.Authority = authConfig.AuthorizationUrl;
                        opts.Audience = "maw_api";

                        opts.TokenValidationParameters = new TokenValidationParameters
                        {
                            NameClaimType = "name"
                        };

                        // https://damienbod.com/2017/10/16/securing-an-angular-signalr-client-using-jwt-tokens-with-asp-net-core-and-identityserver4/
                        opts.Events = new JwtBearerEvents {
                            OnMessageReceived = context =>
                            {
                                if (context.Request.Path.Value.StartsWith("/uploadr") &&
                                    context.Request.Query.TryGetValue("token", out StringValues token)
                                )
                                {
                                    context.Token = token;
                                }

                                return Task.CompletedTask;
                            },
                            OnAuthenticationFailed = context =>
                            {
                                var te = context.Exception;
                                return Task.CompletedTask;
                            }
                        };
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
            app.UseSignalR(routes => routes.MapHub<UploadHub>("/uploadr"));
            app.UseMvc();
        }
    }
}
