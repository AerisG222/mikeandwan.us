using System.Globalization;
using StackExchange.Redis;

namespace Maw.Cache;

abstract class BaseSerializer<T>
    : ISerializer<T>
{
    protected static string GetSortExternalLookup(string hashKeyPattern, string fieldKey) => $"{hashKeyPattern}->{fieldKey}";

    public IEnumerable<T> Parse(RedisValue[] values)
    {
        for(var offset = 0; offset < values.Length; offset += SortLookupFields.Length)
        {
            yield return ParseSingleInternal(new Span<RedisValue>(values, offset, SortLookupFields.Length));
        }
    }

    public T? ParseSingleOrDefault(RedisValue[] values)
    {
        return values.Length == 0 ? default : ParseSingleInternal(new Span<RedisValue>(values, 0, SortLookupFields.Length));
    }

    protected static string SerializeDate(DateTime date)
    {
        return date.ToString("o", CultureInfo.InvariantCulture);
    }

    protected static DateTime DeserializeDate(string value)
    {
        return DateTime.Parse(value, CultureInfo.InvariantCulture);
    }

    public abstract RedisValue[] HashFields { get; }
    public abstract RedisValue[] SortLookupFields { get; }
    public abstract HashEntry[] BuildHashSet(T item);

    protected abstract T ParseSingleInternal(ReadOnlySpan<RedisValue> values);
}
