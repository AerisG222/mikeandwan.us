using StackExchange.Redis;
using Maw.Cache.Abstractions;

namespace Maw.Cache;

public abstract class BaseCache
    : IBaseCache
{
    protected IDatabase Db { get; init; }
    protected string StatusKey { get; }

    public BaseCache(IDatabase redisDatabase, string statusKey)
    {
        Db = redisDatabase ?? throw new ArgumentNullException(nameof(redisDatabase));
        StatusKey = statusKey ?? throw new ArgumentNullException(nameof(statusKey));
    }

    public async Task<CacheStatus> GetStatusAsync()
    {
        var tran = Db.CreateTransaction();
        var status = GetStatusAsync(tran);

        await tran.ExecuteAsync();

        return await status;
    }

    public Task SetStatusAsync(CacheStatus status)
    {
        return ExecuteAsync(tran =>
        {
            tran.StringSetAsync(StatusKey, status.ToString());
        });
    }

    protected Task<CacheStatus> GetStatusAsync(ITransaction tran)
    {
        var task = tran.StringGetAsync(StatusKey);

        return task.ContinueWith(status => {
            if(Enum.TryParse(task.Result, true, out CacheStatus result))
            {
                return result;
            }

            return CacheStatus.UnInitialized;
        });
    }

    protected async Task ExecuteAsync(Action<ITransaction> action)
    {
        var tran = Db.CreateTransaction();

        action(tran);

        var result = await tran.ExecuteAsync();

        if(!result)
        {
            throw new InvalidOperationException("Failed to execute transaction!");
        }
    }

    protected async Task<CacheResult<bool>> IsMemberOfAnySet(RedisValue value, string[] accessSetKeys)
    {
        var tran = Db.CreateTransaction();
        var status = GetStatusAsync(tran);
        var allChecks = new List<Task<bool>>();

        foreach(var key in accessSetKeys)
        {
            allChecks.Add(tran.SetContainsAsync(key, value));
        }

        await tran.ExecuteAsync();

        var isMember = false;

        foreach(var check in allChecks)
        {
            var isInSet = await check;

            if(isInSet)
            {
                isMember = true;
            }
        }

        return BuildResult(await status, isMember);
    }

    protected static CacheResult<T> BuildResult<T>(CacheStatus status, T result)
    {
        var usable = IsStatusUsable(status);

        return new CacheResult<T>(usable, usable ? result : default!);
    }

    protected static bool IsStatusUsable(CacheStatus status)
    {
        return status == CacheStatus.InitializationSucceeded;
    }
}
