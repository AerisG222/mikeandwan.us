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
            var isDevelopment = string.Equals("development", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"), StringComparison.OrdinalIgnoreCase);
            var host = new WebHostBuilder();
            
            if(isDevelopment)
            {
                host
                    .ConfigureLogging(factory =>
                        {
                            factory
                                .AddConsole()
                                .AddFilter(new Dictionary<string, LogLevel>
                                    {
                                        { "Microsoft", LogLevel.Warning },
                                        { "System", LogLevel.Warning },
                                        { "Maw", LogLevel.Debug },
                                        { "MawMvcApp", LogLevel.Debug },
                                    });
                        })
                    .UseKestrel(opts => 
                        {
                            opts.Listen(IPAddress.Loopback, 5000);
                            opts.Listen(IPAddress.Loopback, 5001, listenOptions => {
                                listenOptions.UseHttps("test_certs/testcert.pfx", "TestCertificate");
                            });
                        });
            }
            else
            {
                host
                    /* TODO: add nlog
                    .ConfigureLogging(factory =>
                        {
                            factory.AddNLog();
                        })
                    */
                    .UseKestrel(opts =>
                    {
                        opts.UseSystemd();
                        opts.ListenUnixSocket("/tmp/kestrel.sock");
                    });
            }
            
            host
                .UseContentRoot(Directory.GetCurrentDirectory())
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
