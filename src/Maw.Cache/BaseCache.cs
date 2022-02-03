using StackExchange.Redis;

namespace Maw.Cache;

public abstract class BaseCache
{
    protected IDatabase Db { get; init; }

    public BaseCache(IDatabase redisDatabase)
    {
        Db = redisDatabase ?? throw new ArgumentNullException(nameof(redisDatabase));
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
}
