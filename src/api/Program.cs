using System.IO;
using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


namespace MawApi
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
//                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration((context, builder) =>
                    {
                        builder.AddEnvironmentVariables("MAW_API_");
                    })
                .ConfigureLogging((context, factory) =>
                    {
                        if(context.HostingEnvironment.IsDevelopment())
                        {
                            factory
                                .AddConsole()
                                .AddFilter("Microsoft", LogLevel.Debug)
                                .AddFilter("System", LogLevel.Debug)
                                .AddFilter("Maw", LogLevel.Debug)
                                .AddFilter("MawApi", LogLevel.Debug);
                        }
                        else
                        {
                            factory
                                .AddConsole()
                                .AddFilter("Microsoft", LogLevel.Warning)
                                .AddFilter("System", LogLevel.Warning)
                                .AddFilter("Maw", LogLevel.Information)
                                .AddFilter("MawApi", LogLevel.Information);
                        }
                    })
                .UseDefaultServiceProvider((context, options) => {
                    options.ValidateScopes = context.HostingEnvironment.IsDevelopment();
                })
                .ConfigureWebHostDefaults(webBuilder => {
                    webBuilder.UseStartup<Startup>();

                    webBuilder.UseKestrel(opts =>
                    {
                        var config = (IConfiguration)opts.ApplicationServices.GetService(typeof(IConfiguration));
                        var pwd = File.ReadAllText($"{config["KestrelPfxFile"]}.pwd").Trim();

                        opts.Listen(IPAddress.Loopback, 5011, listenOptions =>
                            {
                                listenOptions.UseHttps(config["KestrelPfxFile"], pwd);
                            });
                    });

                    webBuilder.CaptureStartupErrors(true);
                });
    }
}
