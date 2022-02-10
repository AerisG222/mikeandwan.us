namespace Maw.Cache.Abstractions;

public interface IBaseCache
{
    Task<CacheStatus> GetStatusAsync();
    Task SetStatusAsync(CacheStatus status);
}
