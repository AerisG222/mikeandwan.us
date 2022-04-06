using System.Net;

namespace MawApi;

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
                    builder.AddJsonFile("appsettings.json", true);
                    builder.AddJsonFile("appsettings.{Environment}.json", true);
                    builder.AddEnvironmentVariables("MAW_API_");
                })
            .UseDefaultServiceProvider((context, options) => {
                options.ValidateScopes = context.HostingEnvironment.IsDevelopment();
            })
            .ConfigureWebHostDefaults(webBuilder => {
                webBuilder
                    .CaptureStartupErrors(true)
                    .UseKestrel(opts =>
                    {
                        var config = (IConfiguration?) opts.ApplicationServices.GetService(typeof(IConfiguration));

                        if(config == null)
                        {
                            throw new InvalidOperationException("Config was not found in application services");
                        }

                        var pwd = File.ReadAllText($"{config["KestrelPfxFile"]}.pwd").Trim();

                        opts.Listen(IPAddress.Any, 5011, listenOptions =>
                            {
                                listenOptions.UseHttps(config["KestrelPfxFile"], pwd);
                            });
                    })
                    .UseStartup<Startup>();
            });
}
