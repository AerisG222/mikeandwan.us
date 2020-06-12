using System.IO;
using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


namespace MawAuth
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args)
                .Build()
                .Run();
        }


        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host
                .CreateDefaultBuilder(args)
                .UseSystemd()
                .ConfigureAppConfiguration((context, builder) =>
                    {
                        builder.AddEnvironmentVariables("MAW_AUTH_");
                    })
                .ConfigureLogging((context, factory) =>
                    {
                        if(context.HostingEnvironment.IsDevelopment())
                        {
                            factory
                                .AddConsole()
                                .AddFilter("IdentityServer4", LogLevel.Information)
                                .AddFilter("Microsoft", LogLevel.Information)
                                .AddFilter("System", LogLevel.Information)
                                .AddFilter("Maw", LogLevel.Debug)
                                .AddFilter("MawAuth", LogLevel.Debug);
                        }
                        else
                        {
                            factory
                                .AddConsole()
                                .AddFilter("IdentityServer4", LogLevel.Warning)
                                .AddFilter("Microsoft", LogLevel.Warning)
                                .AddFilter("System", LogLevel.Warning)
                                .AddFilter("Maw", LogLevel.Warning)
                                .AddFilter("MawAuth", LogLevel.Warning);
                        }
                    })
                .UseDefaultServiceProvider((context, options) => {
                    options.ValidateScopes = context.HostingEnvironment.IsDevelopment();
                })
                .ConfigureWebHostDefaults(webBuilder => {
                    webBuilder
                        .CaptureStartupErrors(true)
                        .UseKestrel(opts =>
                        {
                            opts.Listen(IPAddress.Loopback, 5001, listenOptions =>
                                {
                                    var config = (IConfiguration)opts.ApplicationServices.GetService(typeof(IConfiguration));
                                    var pwd = File.ReadAllText($"{config["KestrelPfxFile"]}.pwd").Trim();

                                    listenOptions.UseHttps(config["KestrelPfxFile"], pwd);
                                });
                        })
                        .UseStartup<Startup>();
                });
    }
}
