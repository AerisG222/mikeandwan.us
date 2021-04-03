using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using NWebsec.Core.Common.Middleware.Options;
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


        public Startup(IConfiguration config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }


        public void ConfigureServices(IServiceCollection services)
        {
            var urlConfig = new UrlConfig();

            _config.GetSection("UrlConfig").Bind(urlConfig);

            ConfigureDataProtection(services);

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
                .AddResponseCompression()
                .AddControllers()
                    .Services
                .AddSignalR()
                    .Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(opts => {
                        opts.Authority = urlConfig.Auth;
                        opts.Audience = "maw_api_resource";

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
                    opts.AddDefaultPolicy(policy => {
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


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if(env.IsProduction())
            {
                app.UseHsts(hsts => hsts.MaxAge(365 * 2).IncludeSubdomains().Preload());
            }
            else
            {
                IdentityModelEventSource.ShowPII = true;
            }

            app
                .UseXContentTypeOptions()
                .UseReferrerPolicy(opts => opts.StrictOriginWhenCrossOrigin())

                .UseResponseCompression()
                .UseNoCacheHttpHeaders()
                .UseXfo(xfo => xfo.Deny())
                .UseXXssProtection(opts => opts.EnabledWithBlockMode())
                .UseRedirectValidation()
                .UseCsp(DefineContentSecurityPolicy)

                .UseRouting()
                .UseCors()
                .UseOpenApi()
                .UseReDoc()
                .UseAuthentication()
                .UseAuthorization()
                .UseEndpoints(endpoints => {
                    endpoints.MapHub<UploadHub>("/uploadr");
                    endpoints.MapControllers();
                });
        }


        void ConfigureDataProtection(IServiceCollection services)
        {
            var dpPath = _config["DataProtection:Path"];

            if(!string.IsNullOrWhiteSpace(dpPath))
            {
                services
                    .AddDataProtection()
                    .PersistKeysToFileSystem(new DirectoryInfo(dpPath));
            }
        }

        void DefineContentSecurityPolicy(IFluentCspOptions csp)
        {
            var fontSources = new string[] {
                "https://fonts.gstatic.com"
            };

            var imageSources = new string[] {
                "data:"
            };

            var scriptSources = new string[] {
                "https://cdn.jsdelivr.net"
            };

            var styleSources = new string[] {
                "https://fonts.googleapis.com"
            };

            var workerSources = new string[] {
                "blob:"
            };

            csp
                .DefaultSources(s => s.None())
                .BaseUris(s => s.Self())
                .ConnectSources(s => s.Self())
                .FontSources(s => s.CustomSources(fontSources))
                .ImageSources(s => s.CustomSources(imageSources))
                .ScriptSources(s => {
                    s.UnsafeInline();
                    s.CustomSources(scriptSources);
                })
                .StyleSources(s => {
                    s.UnsafeInline();
                    s.CustomSources(styleSources);
                })
                .WorkerSources(s => s.CustomSources(workerSources));
        }
    }
}
