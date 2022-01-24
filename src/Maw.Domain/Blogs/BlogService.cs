using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Maw.Domain.Blogs;

public class BlogService
    : BaseService, IBlogService
{
    readonly IBlogRepository _repo;

    public BlogService(
        IBlogRepository blogRepository,
        ILogger<BlogService> log,
        IDistributedCache cache)
        : base("blog", log, cache)
    {
        _repo = blogRepository ?? throw new ArgumentNullException(nameof(blogRepository));
    }

    public async Task<IEnumerable<Blog>> GetBlogsAsync()
    {
        var blogs = await GetCachedValueAsync(nameof(GetBlogsAsync), () => _repo.GetBlogsAsync());

        return blogs ?? new List<Blog>();
    }

    public async Task<IEnumerable<Post>> GetAllPostsAsync(short blogId)
    {
        var key = $"{nameof(GetAllPostsAsync)}_{blogId}";
        var posts = await GetCachedValueAsync(key, () => _repo.GetAllPostsAsync(blogId));

        return posts ?? new List<Post>();
    }

    public async Task<IEnumerable<Post>> GetLatestPostsAsync(short blogId, short postCount)
    {
        var key = $"{nameof(GetLatestPostsAsync)}_{blogId}_{postCount}";
        var posts = await GetCachedValueAsync(key, () => _repo.GetLatestPostsAsync(blogId, postCount));

        return posts ?? new List<Post>();
    }

    public Task AddPostAsync(Post post)
    {
        if (post == null)
        {
            throw new ArgumentNullException(nameof(post));
        }

        return Task.WhenAll(
            _repo.AddPostAsync(post),
            InternalClearCacheAsync()
        );
    }
}
