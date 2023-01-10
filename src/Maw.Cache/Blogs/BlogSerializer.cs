using StackExchange.Redis;
using Maw.Domain.Models.Blogs;

namespace Maw.Cache.Blogs;

sealed class BlogSerializer
    : BaseSerializer<Blog>
{
    const string KEY_ID = "id";
    const string KEY_TITLE = "title";
    const string KEY_COPYRIGHT = "copyright";
    const string KEY_DESCRIPTION = "description";
    const string KEY_LAST_POST_DATE = "last-post-date";

    static readonly RedisValue[] _hashFields = new RedisValue[]
    {
        KEY_ID,
        KEY_TITLE,
        KEY_COPYRIGHT,
        KEY_DESCRIPTION,
        KEY_LAST_POST_DATE
    };

    static readonly RedisValue[] _sortLookup = new RedisValue[]
    {
        "#",
        GetSortExternalLookup(BlogKeys.BLOG_HASH_KEY_PATTERN, KEY_TITLE),
        GetSortExternalLookup(BlogKeys.BLOG_HASH_KEY_PATTERN, KEY_COPYRIGHT),
        GetSortExternalLookup(BlogKeys.BLOG_HASH_KEY_PATTERN, KEY_DESCRIPTION),
        GetSortExternalLookup(BlogKeys.BLOG_HASH_KEY_PATTERN, KEY_LAST_POST_DATE)
    };

    public override RedisValue[] HashFields { get => _hashFields; }
    public override RedisValue[] SortLookupFields { get => _sortLookup; }

    public override HashEntry[] BuildHashSet(Blog item)
    {
        return new HashEntry[]
        {
            new HashEntry(KEY_ID, item.Id),
            new HashEntry(KEY_TITLE, item.Title),
            new HashEntry(KEY_COPYRIGHT, item.Copyright),
            new HashEntry(KEY_DESCRIPTION, item.Description),
            new HashEntry(KEY_LAST_POST_DATE, SerializeDate(item.LastPostDate))
        };
    }

    protected override Blog ParseSingleInternal(ReadOnlySpan<RedisValue> values)
    {
        return new Blog
        {
            Id = (short)values[0],
            Title = values[1]!,
            Copyright = values[2]!,
            Description = values[3]!,
            LastPostDate = DeserializeDate(values[4]!)
        };
    }
}
