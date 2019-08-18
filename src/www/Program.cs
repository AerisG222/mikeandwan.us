using System.IO;
using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


namespace MawMvcApp
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder();

            host
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration((context, builder) =>
                    {
                        builder.AddJsonFile("config.json");
                        builder.AddEnvironmentVariables("MAW_WWW_");
                    })
                .ConfigureLogging((context, factory) =>
                    {
                        if(context.HostingEnvironment.IsDevelopment())
                        {
                            factory
                                .AddConsole()
                                .AddFilter("Microsoft", LogLevel.Warning)
                                .AddFilter("System", LogLevel.Warning)
                                .AddFilter("Maw", LogLevel.Debug)
                                .AddFilter("MawMvcApp", LogLevel.Debug);
                        }
                        else
                        {
                            factory
                                .AddConsole()
                                .AddFilter("Microsoft", LogLevel.Warning)
                                .AddFilter("System", LogLevel.Warning)
                                .AddFilter("Maw", LogLevel.Information)
                                .AddFilter("MawMvcApp", LogLevel.Information);
                        }
                    })
                .UseKestrel(opts =>
                    {
                        opts.Listen(IPAddress.Loopback, 5021, listenOptions =>
                            {
                                var config = (IConfiguration)opts.ApplicationServices.GetService(typeof(IConfiguration));
                                var pwd = File.ReadAllText($"{config["KestrelPfxFile"]}.pwd").Trim();

                                listenOptions.UseHttps(config["KestrelPfxFile"], pwd);
                            });
                    })
                .CaptureStartupErrors(true)
                .UseDefaultServiceProvider((context, options) => {
                    options.ValidateScopes = context.HostingEnvironment.IsDevelopment();
                })
                .UseStartup<Startup>()
                .Build()
                .Run();
        }
    }
}
