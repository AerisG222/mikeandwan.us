using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;


namespace MawMvcApp
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
                            opts.Listen(IPAddress.Loopback, 5000);
                            opts.Listen(IPAddress.Loopback, 5001, listenOptions => {
                                listenOptions.UseHttps("test_certs/testcert.pfx", "TestCertificate");
                            });
                        }
                        else if(isStaging)
                        {
                            opts.Listen(IPAddress.Loopback, 5000);
                        }
                        else
                        {
                            opts.UseSystemd();
                            opts.ListenUnixSocket("/var/run/mikeandwan.us/kestrel.sock");
                            opts.Listen(IPAddress.Loopback, 5000);
                        }
                    })
                .ConfigureAppConfiguration((context, builder) =>
                    {
                        builder.AddJsonFile("config.json");
                        builder.AddEnvironmentVariables("MAW_");
                    })
                .CaptureStartupErrors(true)
                .UseStartup<Startup>()
                .Build()
                .Run();
        }
    }
}
