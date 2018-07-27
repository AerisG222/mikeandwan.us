﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;


namespace MawAuth
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder();

            host
                .UseContentRoot(Directory.GetCurrentDirectory())
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
                                .AddFilter("IdentityServer4", LogLevel.Debug)
                                .AddFilter("Microsoft", LogLevel.Warning)
                                .AddFilter("System", LogLevel.Warning)
                                .AddFilter("Maw", LogLevel.Debug)
                                .AddFilter("MawAuth", LogLevel.Debug);
                        }
                        else
                        {
                            factory
                                .AddConsole()
                                .AddFilter("IdentityServer4", LogLevel.Debug)
                                .AddFilter("Microsoft", LogLevel.Warning)
                                .AddFilter("System", LogLevel.Warning)
                                .AddFilter("Maw", LogLevel.Debug)
                                .AddFilter("MawAuth", LogLevel.Debug);
                        }
                    })
                .UseKestrel(opts =>
                    {
                        opts.Listen(IPAddress.Loopback, 5001, listenOptions =>
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
                .UseLinuxTransport()
                .UseStartup<Startup>()
                .Build()
                .Run();
        }
    }
}
