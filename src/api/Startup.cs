using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using NMagickWand;
using SolrNet;
using Maw.Data;
using Maw.Domain;
using Maw.Domain.Search;
using Maw.Domain.Upload;
using Maw.Security;
using MawApi.Hubs;
using MawApi.Services;
using MawApi.Models;


namespace MawApi
{
    public class Startup
    {
        readonly IConfiguration _config;
        readonly IWebHostEnvironment _env;


        public Startup(IConfiguration config, IWebHostEnvironment env)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _env = env ?? throw new ArgumentNullException(nameof(env));

            MagickWandEnvironment.Genesis();
        }


        public void ConfigureServices(IServiceCollection services)
        {
            var urlConfig = new UrlConfig();

            _config.GetSection("UrlConfig").Bind(urlConfig);

            services
                .Configure<EnvironmentConfig>(_config.GetSection("Environment"))
                .Configure<UploadConfig>(_config.GetSection("FileUpload"))
                .AddSingleton<UrlConfig>(urlConfig)
                .AddMawDataServices(_config["Environment:DbConnectionString"])
                .AddSolrNet<MultimediaCategory>(_config["Search:CoreUrl"])
                .AddMawDomainServices()
                .AddMawApiServices()
                .AddScoped<ISearchService, SearchService>()
                .AddScoped<IContentTypeProvider, FileExtensionContentTypeProvider>()
                .AddControllers()
                    .Services
                .AddSignalR()
                    .Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(opts => {
                        opts.Authority = urlConfig.Auth;
                        opts.Audience = "maw_api";

                        opts.TokenValidationParameters = new TokenValidationParameters
                        {
                            NameClaimType = "name"
                        };

                        // https://damienbod.com/2017/10/16/securing-an-angular-signalr-client-using-jwt-tokens-with-asp-net-core-and-identityserver4/
                        opts.Events = new JwtBearerEvents {
                            OnMessageReceived = context =>
                            {
                                if (context.Request.Path.Value.StartsWith("/uploadr", true, CultureInfo.InvariantCulture) &&
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
                        var origins = new string[] {
                            urlConfig.Www,
                            urlConfig.Photos,
                            urlConfig.Files
                        };

                        policy.WithOrigins(origins)
                            .WithExposedHeaders(new string[] {
                                "Content-Disposition"
                            })
                            .AllowCredentials()
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
                })
                .AddOpenApiDocument(doc => {
                    doc.Title = "mikeandwan.us APIs";
                    doc.Description = "Full suite of APIs for interacting with data hosted at mikeandwan.us";
                });
        }


        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseCors("default");
            app.UseOpenApi();
            app.UseSwaggerUi3();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => {
                endpoints.MapHub<UploadHub>("/uploadr");
                endpoints.MapControllers();
            });
        }
    }
}
