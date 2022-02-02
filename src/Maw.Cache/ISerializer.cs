using StackExchange.Redis;

namespace Maw.Cache;

interface ISerializer<T>
{
    RedisValue[] SortLookupFields { get; }

    IEnumerable<T> Parse(RedisValue[] values);
    T? ParseSingleOrDefault(RedisValue[] values);

    HashEntry[] BuildHashSet(T item);
}
