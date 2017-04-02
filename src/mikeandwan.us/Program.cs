using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;


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
                    .UseKestrel(opts => 
                    {
                        opts.UseHttps("test_certs/testcert.pfx", "TestCertificate");
                    })
                    .UseUrls("http://localhost:5000", "https://localhost:5001");
            }
            else
            {
                host.UseKestrel();
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
