using StackExchange.Redis;
using Maw.Domain.Models.Blogs;

namespace Maw.Cache.Blogs;

class PostSerializer
    : BaseSerializer<Post>
{
    const string KEY_ID = "id";
    const string KEY_BLOG_ID = "blog-id";
    const string KEY_TITLE = "title";
    const string KEY_DESCRIPTION = "description";
    const string KEY_PUBLISH_DATE = "publish-date";

    static readonly RedisValue[] _hashFields = new RedisValue[]
    {
        KEY_ID,
        KEY_BLOG_ID,
        KEY_TITLE,
        KEY_DESCRIPTION,
        KEY_PUBLISH_DATE
    };

    static readonly RedisValue[] _sortLookup = new RedisValue[]
    {
        "#",
        GetSortExternalLookup(BlogKeys.POST_HASH_KEY_PATTERN, KEY_BLOG_ID),
        GetSortExternalLookup(BlogKeys.POST_HASH_KEY_PATTERN, KEY_TITLE),
        GetSortExternalLookup(BlogKeys.POST_HASH_KEY_PATTERN, KEY_DESCRIPTION),
        GetSortExternalLookup(BlogKeys.POST_HASH_KEY_PATTERN, KEY_PUBLISH_DATE)
    };

    public override RedisValue[] HashFields { get => _hashFields; }
    public override RedisValue[] SortLookupFields { get => _sortLookup; }

    public override HashEntry[] BuildHashSet(Post item)
    {
        return new HashEntry[]
        {
            new HashEntry(KEY_ID, item.Id),
            new HashEntry(KEY_BLOG_ID, item.BlogId),
            new HashEntry(KEY_TITLE, item.Title),
            new HashEntry(KEY_DESCRIPTION, item.Description),
            new HashEntry(KEY_PUBLISH_DATE, SerializeDate(item.PublishDate))
        };
    }

    protected override Post ParseSingleInternal(ReadOnlySpan<RedisValue> values)
    {
        return new Post
        {
            Id = (short)values[0],
            BlogId = (short)values[1],
            Title = values[2],
            Description = values[3],
            PublishDate = DeserializeDate(values[4])
        };
    }
}
