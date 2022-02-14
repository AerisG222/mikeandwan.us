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
    protected async Task<T?> GetNullableCachedValueAsync<T>(Func<Task<CacheResult<T>>> getFromCache, Func<Task<T?>> getFromRepo)
    {
        var val = await TryCacheAsync(getFromCache);

        if(val.ShouldUseResult)
        {
            return val.Value;
        }

        return await getFromRepo();
    }

    protected async Task<T> GetCachedValueAsync<T>(Func<Task<CacheResult<T>>> getFromCache, Func<Task<T>> getFromRepo)
    {
        var val = await TryCacheAsync(getFromCache);

        if(val.ShouldUseResult)
        {
            return val.Value;
        }

        return await getFromRepo();
    }

    protected async Task<T?> GetOrSetCachedValueAsync<T>(Func<Task<T?>> getFromCache, Func<Task<T?>> getFromRepo, Func<T, Task> setInCache)
    {
        var value = await TryCacheAsync(getFromCache);

        if(value == null)
        {
            value = await getFromRepo();

            if(value != null)
            {
                await TryCacheAsync(() => setInCache(value));
            }
        }

        return value;
    }

    protected async Task TryCacheAsync(Func<Task> useCache)
    {
        try
        {
            await useCache();
        }
        catch(Exception ex)
        {
            Log.LogError(ex, "Error trying to use cache");
        }
    }

    protected async Task<T?> TryCacheAsync<T>(Func<Task<T>> useCache)
    {
        try
        {
            return await useCache();
        }
        catch(Exception ex)
        {
            Log.LogError(ex, "Error trying to use cache");
        }

        return default;
    }

    protected async Task<CacheResult<T>> TryCacheAsync<T>(Func<Task<CacheResult<T>>> useCache)
    {
        try
        {
            return await useCache();
        }
        catch(Exception ex)
        {
            Log.LogError(ex, "Error trying to use cache");
        }

        return new CacheResult<T>(false, default);
    }
}
