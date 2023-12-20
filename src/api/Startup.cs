﻿using System.Globalization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Logging;
using NSwag.Generation.AspNetCore;
using NWebsec.Core.Common.Middleware.Options;
using SolrNet;
using Maw.Cache;
using Maw.Data;
using Maw.Domain;
using Maw.Domain.Models.Search;
using Maw.Domain.Search;
using Maw.Domain.Upload;
using Maw.Security;
using MawApi.Hubs;
using MawApi.Services;
using MawApi.Models;
using IdentityModel;

namespace MawApi;

public class Startup
{
    static readonly string[] CORS_HEADERS_DEFAULT = [
        "Content-Disposition"
    ];

    readonly IConfiguration _config;

    public Startup(IConfiguration config)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
    }

    public void ConfigureServices(IServiceCollection services)
    {
        var urlConfig = _config.GetSection("UrlConfig").Get<UrlConfig>() ?? throw new InvalidOperationException("url config should not be null!");

        ConfigureDataProtection(services);

        services
            .Configure<EnvironmentConfig>(_config.GetSection("Environment"))
            .Configure<UploadConfig>(_config.GetSection("FileUpload"))
            .Configure<UrlConfig>(_config.GetSection("UrlConfig"))
            .AddMawCacheServices(_config["Environment:RedisConnectionString"] ?? throw new InvalidOperationException("redis connection string config cannot be null!"))
            .AddMawDataServices(_config["Environment:DbConnectionString"] ?? throw new InvalidOperationException("db connection string config cannot be null!"))
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
                .AddJwtBearer(opts => ConfigureJwtBearerOptions(opts, urlConfig))
                .Services
            .AddAuthorization(opts => MawPolicyBuilder.AddMawPolicies(opts))
            .AddCors(opts => ConfigureDefaultCorsPolicy(opts, urlConfig))
            .AddOpenApiDocument(doc => ConfigureOpenApiDocumentOptions(doc));
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
            .UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            })
            .UseCors()
            .UseOpenApi(opts => {
                opts.DocumentName = "openapi";
                opts.Path = "/openapi/v1/openapi.json";
            })
            .UseReDoc(opts => {
                opts.Path = "/openapi";
                opts.DocumentPath = "/openapi/v1/openapi.json";
            })
            .UseAuthentication()
            .UseAuthorization()
            .UseEndpoints(endpoints => {
                endpoints.MapHub<UploadHub>("/uploadr");
                endpoints.MapControllers();
            });
    }

    static void ConfigureOpenApiDocumentOptions(AspNetCoreOpenApiDocumentGeneratorSettings doc)
    {
        doc.DocumentName = "openapi";
        doc.Title = "mikeandwan.us APIs";
        doc.Description = "Full suite of APIs for interacting with data hosted at mikeandwan.us";
    }

    static void ConfigureJwtBearerOptions(JwtBearerOptions opts, UrlConfig urlConfig)
    {
        opts.Authority = urlConfig.Auth;
        opts.Audience = "maw_api_resource";
        opts.TokenValidationParameters.NameClaimType = JwtClaimTypes.Name;

        // https://damienbod.com/2017/10/16/securing-an-angular-signalr-client-using-jwt-tokens-with-asp-net-core-and-identityserver4/
        opts.Events = new JwtBearerEvents {
            OnMessageReceived = context =>
            {
                var isUploadr = context.Request.Path.Value?.StartsWith("/uploadr", true, CultureInfo.InvariantCulture) ?? false;

                if (isUploadr && context.Request.Query.TryGetValue("token", out StringValues token))
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
    }

    static void ConfigureDefaultCorsPolicy(CorsOptions opts, UrlConfig urlConfig)
    {
        opts.AddDefaultPolicy(policy => {
#pragma warning disable CA1861
            var origins = new string[] {
                urlConfig.Www,
                urlConfig.Photos,
                urlConfig.Files,
                urlConfig.PhotosSolid
            };
#pragma warning restore CA1861

            policy.WithOrigins(origins)
                .WithExposedHeaders(CORS_HEADERS_DEFAULT)
                .AllowCredentials()
                .AllowAnyHeader()
                .AllowAnyMethod();
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
