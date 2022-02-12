using Microsoft.Extensions.Logging;
using Maw.Cache.Abstractions;

namespace Maw.Domain;

public class BaseService
{
    protected ILogger Log { get; init; }

    public BaseService(ILogger log)
    {
        Log = log ?? throw new ArgumentNullException(nameof(log));
    }

    // TODO: is there a better way to normalize these functions?
    protected static async Task<T?> GetNullableCachedValueAsync<T>(Func<Task<CacheResult<T>>> getFromCache, Func<Task<T?>> getFromRepo)
    {
        var val = await getFromCache();

        if(val.ShouldUseResult)
        {
            return val.Value;
        }

        return await getFromRepo();
    }

    protected static async Task<T> GetCachedValueAsync<T>(Func<Task<CacheResult<T>>> getFromCache, Func<Task<T>> getFromRepo)
    {
        var val = await getFromCache();

        if(val.ShouldUseResult)
        {
            return val.Value;
        }

        return await getFromRepo();
    }
}
