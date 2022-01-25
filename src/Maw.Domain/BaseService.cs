using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Maw.Domain;

public class BaseService
{
    readonly TimeSpan _maxCacheTime = TimeSpan.FromDays(365);
    readonly string _cachePrefix;
    readonly IDistributedCache _cache;
    readonly ILogger _log;

    public BaseService(string cachePrefix,
                       ILogger log,
                       IDistributedCache cache)
    {
        _cachePrefix = cachePrefix ?? throw new ArgumentNullException(nameof(cachePrefix));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _log = log ?? throw new ArgumentNullException(nameof(log));
    }

    protected async Task<T?> GetCachedValueAsync<T>(string key, Func<Task<T>> func, TimeSpan? slidingExpiration = null)
    {
        if (string.IsNullOrEmpty(key))
        {
            throw new ArgumentNullException(nameof(key));
        }

        if (func == null)
        {
            throw new ArgumentNullException(nameof(func));
        }

        var normalizedKey = NormalizeKey(key);
        var cachedValue = await _cache.GetAsync(normalizedKey);

        if (cachedValue == null)
        {
            var result = await func();

            await SetCachedValueAsync(normalizedKey, result, slidingExpiration);
            await AddCachedKeyAsync(normalizedKey);

            return result;
        }

        return JsonSerializer.Deserialize<T>(cachedValue);
    }

    protected async Task InternalClearCacheAsync()
    {
        var keys = await GetCachedKeysAsync();

        if (keys == null)
        {
            return;
        }

        foreach (var key in keys)
        {
            _log.LogDebug("Removing cache entry with key: {Key}", key);

            await _cache.RemoveAsync(key);
        }

        await ClearCachedKeysAsync();
    }

    string NormalizeKey(string key)
    {
        return $"{_cachePrefix}_{key}";
    }

    async Task<List<string>?> GetCachedKeysAsync()
    {
        var cachedValue = await _cache.GetStringAsync(BuildCachedKeysKey());

        if (cachedValue == null)
        {
            return null;
        }

        return JsonSerializer.Deserialize<List<string>>(cachedValue);
    }

    Task ClearCachedKeysAsync()
    {
        return _cache.RemoveAsync(BuildCachedKeysKey());
    }

    string BuildCachedKeysKey()
    {
        return $"{_cachePrefix}_CACHED_KEYS";
    }

    Task SetCachedValueAsync<T>(string key, T value, TimeSpan? slidingExpiration)
    {
        var opts = new DistributedCacheEntryOptions { SlidingExpiration = slidingExpiration ?? _maxCacheTime };

        _log.LogDebug("Setting cache entry with key: {Key}", key);

        return _cache.SetStringAsync(key, JsonSerializer.Serialize(value), opts);
    }

    async Task AddCachedKeyAsync(string key)
    {
        var keys = await GetCachedKeysAsync();

        if (keys == null)
        {
            keys = new List<string> { key };

            await SetCachedValueAsync(BuildCachedKeysKey(), keys, null);
        }
        else
        {
            if (!keys.Contains(key))
            {
                keys.Add(key);

                await SetCachedValueAsync(BuildCachedKeysKey(), keys, null);
            }
        }
    }
}
