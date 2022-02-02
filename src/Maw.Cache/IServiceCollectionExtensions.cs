using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using Maw.Cache.Abstractions;
using Maw.Cache.Blogs;
using Maw.Cache.Photos;
using Maw.Cache.Videos;

namespace Maw.Cache;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddMawCacheServices(this IServiceCollection services, string redisConnectionString)
    {
        return services
            .AddSingleton<IConnectionMultiplexer, ConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisConnectionString))
            .AddScoped(services => services.GetRequiredService<ConnectionMultiplexer>().GetDatabase())
            .AddScoped<IBlogCache, BlogCache>()
            .AddScoped<IPhotoCache, PhotoCache>()
            .AddScoped<IVideoCache, VideoCache>();
    }
}
