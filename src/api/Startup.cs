using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Maw.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
            services.AddMvcCore()
                .AddAuthorization()
                .AddJsonFormatters();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(opts => {
                    opts.Authority = "https://localhost:5001";
                    opts.RequireHttpsMetadata = false;

                    opts.ApiName = "admin";
                })
                .Services
                .AddAuthorization(opts =>
                    {
                        MawPolicyBuilder.AddMawPolicies(opts);
                    });
        }


        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseAuthentication();

            app.UseMvc();
        }
    }
}
