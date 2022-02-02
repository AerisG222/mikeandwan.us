using StackExchange.Redis;
using Maw.Cache.Abstractions;
using Maw.Domain.Models.Blogs;

namespace Maw.Cache.Blogs;

public class BlogCache
    : BaseCache, IBlogCache
{
    readonly BlogSerializer _blogSerializer = new();
    readonly PostSerializer _postSerializer = new();

    public BlogCache(IDatabase redisDatabase)
        : base(redisDatabase)
    {

    }

    public async Task<IEnumerable<Blog>> GetBlogsAsync()
    {
        var tran = Db.CreateTransaction();

        var blogs = tran.SortAsync(
            BlogKeys.ALL_BLOGS_SET_KEY,
            get: _blogSerializer.SortLookupFields
        );

        await tran.ExecuteAsync();

        return _blogSerializer.Parse(await blogs);
    }

    public Task AddBlogsAsync(IEnumerable<Blog> blogs)
    {
        return ExecuteAsync(tran =>
        {
            foreach(var blog in blogs)
            {
                tran.HashSetAsync(
                    BlogKeys.GetBlogHashKey(blog),
                    _blogSerializer.BuildHashSet(blog)
                );

                tran.SetAddAsync(
                    BlogKeys.ALL_BLOGS_SET_KEY,
                    blog.Id
                );
            }
        });
    }

    public Task AddBlogAsync(Blog blog)
    {
        return AddBlogsAsync(new Blog[] { blog });
    }

    public async Task<IEnumerable<Post>> GetPostsAsync(short blogId, long? count = null)
    {
        var tran = Db.CreateTransaction();

        var posts = tran.SortAsync(
            BlogKeys.GetBlogPostsKey(blogId),
            order: Order.Descending,
            take: count ?? -1,
            get: _postSerializer.SortLookupFields
        );

        await tran.ExecuteAsync();

        return _postSerializer.Parse(await posts);
    }

    public Task AddPostsAsync(IEnumerable<Post> posts)
    {
        return ExecuteAsync(tran =>
        {
            foreach(var post in posts)
            {
                tran.HashSetAsync(
                    BlogKeys.GetPostHashKey(post),
                    _postSerializer.BuildHashSet(post)
                );

                tran.SetAddAsync(
                    BlogKeys.GetBlogPostsKey(post.BlogId),
                    post.Id
                );
            }
        });
    }

    public Task AddPostAsync(Post post)
    {
        return AddPostsAsync(new Post[] { post });
    }
}
