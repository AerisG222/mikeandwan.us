using Microsoft.Extensions.Logging;
using Maw.Cache.Abstractions;
using Maw.Data.Abstractions;
using Maw.Domain.Models.Blogs;

namespace Maw.Domain.Blogs;

public class BlogService
    : BaseService, IBlogService
{
    readonly IBlogRepository _repo;
    readonly IBlogCache _cache;

    public BlogService(
        IBlogRepository repo,
        IBlogCache cache,
        ILogger<BlogService> log)
        : base(log)
    {
        _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    public async Task<IEnumerable<Blog>> GetBlogsAsync()
    {
        var blogs = await GetCachedValueAsync(
            () => _cache.GetBlogsAsync(),
            () => _repo.GetBlogsAsync()
        );

        return blogs ?? new List<Blog>();
    }

    public async Task<IEnumerable<Post>> GetAllPostsAsync(short blogId)
    {
        var posts = await GetCachedValueAsync(
            () => _cache.GetPostsAsync(blogId, null),
            () => _repo.GetAllPostsAsync(blogId)
        );

        return posts ?? new List<Post>();
    }

    public async Task<IEnumerable<Post>> GetLatestPostsAsync(short blogId, short postCount)
    {
        var posts = await GetCachedValueAsync(
            () => _cache.GetPostsAsync(blogId, postCount),
            () => _repo.GetLatestPostsAsync(blogId, postCount)
        );

        return posts ?? new List<Post>();
    }

    public Task AddPostAsync(Post post)
    {
        ArgumentNullException.ThrowIfNull(post);

        return Task.WhenAll(
            _repo.AddPostAsync(post),
            _cache.AddPostAsync(post)
        );
    }
}
