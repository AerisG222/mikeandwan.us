using System;
using System.IO;
using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
                            factory.AddConsole();
                        })
                    .UseKestrel(opts => 
                        {
                            opts.Listen(IPAddress.Loopback, 5000);
                            opts.Listen(IPAddress.Loopback, 5001, listenOptions => {
                                listenOptions.UseHttps("test_certs/testcert.pfx", "TestCertificate");
                            });
                        });
                    //.UseUrls("http://localhost:5000", "https://localhost:5001");
            }
            else
            {
                host
                    /*
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
                .CaptureStartupErrors(true)
                .UseStartup<Startup>()
                .Build()
                .Run();
        }
    }
}
