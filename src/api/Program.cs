using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MawApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder();
            var isDevelopment = false;
            var isStaging = false;

            host
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureLogging((context, factory) =>
                    {
                        isDevelopment = context.HostingEnvironment.IsDevelopment();
                        isStaging = context.HostingEnvironment.IsStaging();

                        if(isDevelopment)
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
                        if(isDevelopment)
                        {
                            opts.Listen(IPAddress.Loopback, 5010);
                            opts.Listen(IPAddress.Loopback, 5011, listenOptions => {
                                listenOptions.UseHttps("certs/api.pfx", "test");
                            });
                        }
                        else if(isStaging)
                        {
                            opts.Listen(IPAddress.Loopback, 5010);
                        }
                        else
                        {
                            opts.UseSystemd();
                            opts.ListenUnixSocket("/var/run/mikeandwan.us/api.sock");
                            opts.Listen(IPAddress.Loopback, 5010);
                        }
                    })
                .ConfigureAppConfiguration((context, builder) =>
                    {
                        builder.AddEnvironmentVariables("MAW_API_");
                    })
                .CaptureStartupErrors(true)
                .UseStartup<Startup>()
                .Build()
                .Run();
        }
    }
}
