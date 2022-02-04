using StackExchange.Redis;

namespace Maw.Cache;

public abstract class BaseCache
{
    protected IDatabase Db { get; init; }
    protected string StatusKey { get; }

    public BaseCache(IDatabase redisDatabase, string statusKey)
    {
        Db = redisDatabase ?? throw new ArgumentNullException(nameof(redisDatabase));
        StatusKey = statusKey ?? throw new ArgumentNullException(nameof(statusKey));
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

    protected async Task<bool> IsMemberOfAnySet(RedisValue value, string[] accessSetKeys)
    {
        var tran = Db.CreateTransaction();
        var allChecks = new List<Task<bool>>();

        foreach(var key in accessSetKeys)
        {
            allChecks.Add(tran.SetContainsAsync(key, value));
        }

        await tran.ExecuteAsync();

        foreach(var check in allChecks)
        {
            var isInSet = await check;

            if(isInSet)
            {
                return true;
            }
        }

        return false;
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
}
