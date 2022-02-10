using Maw.Cache.Initializer;
using Maw.Cache;
using Maw.Data;

const string ENV_DB = "MAW_WORKER_DB_CONNECTION_STRING";
const string ENV_REDIS = "MAW_WORKER_REDIS_CONNECTION_STRING";

var dbConnString = Environment.GetEnvironmentVariable(ENV_DB);
var redisConnString = Environment.GetEnvironmentVariable(ENV_REDIS);

/*
if(string.IsNullOrWhiteSpace(dbConnString))
{
    throw new InvalidProgramException($"MAW DB connection string is not properly specified in environment variable {ENV_DB}");
}

if(string.IsNullOrWhiteSpace(redisConnString))
{
    throw new InvalidProgramException($"MAW REDIS connection string is not properly specified in environment variable {ENV_REDIS}");
}
*/

IHost host = Host
    .CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services
            .AddMawDataServices(dbConnString!)
            .AddMawCacheServices(redisConnString!)
            .AddSingleton<IDelayCalculator, DelayCalculator>()
            .AddScoped<IScopedProcessingService, BlogCacheProcessingService>()
            .AddScoped<IScopedProcessingService, PhotoCacheProcessingService>()
            .AddScoped<IScopedProcessingService, VideoCacheProcessingService>()
            .AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
