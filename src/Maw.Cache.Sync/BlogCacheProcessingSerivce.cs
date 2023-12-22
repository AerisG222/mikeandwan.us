using System.Diagnostics;
using Maw.Data.Abstractions;
using Maw.Cache.Abstractions;
using Maw.Domain.Models.Blogs;

namespace Maw.Cache.Sync;

internal class BlogCacheProcessingService
    : IScopedProcessingService
{
    const int BASE_DELAY = 60_000;
    const float DELAY_FLUCTUATION_PCT = 0.25f;

    readonly IBlogRepository _repo;
    readonly IBlogCache _cache;
    readonly IDelayCalculator _delay;
    readonly ILogger _logger;

    public BlogCacheProcessingService(
        IBlogRepository repo,
        IBlogCache cache,
        IDelayCalculator delayCalculator,
        ILogger<BlogCacheProcessingService> logger)
    {
        ArgumentNullException.ThrowIfNull(repo);
        ArgumentNullException.ThrowIfNull(cache);
        ArgumentNullException.ThrowIfNull(delayCalculator);
        ArgumentNullException.ThrowIfNull(logger);

        _repo = repo;
        _cache = cache;
        _delay = delayCalculator;
        _logger = logger;
    }

    public async Task DoWorkAsync(CancellationToken stoppingToken)
    {
        var stopwatch = new Stopwatch();

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("{service} running at: {time}", nameof(BlogCacheProcessingService), DateTimeOffset.Now);

            stopwatch.Restart();

            try
            {
                await UpdateBlogCache(stoppingToken);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error updating video cache: {msg}", ex.Message);
            }

            stopwatch.Stop();

            var jitteredDelay = _delay.CalculateRandomizedDelay(BASE_DELAY, DELAY_FLUCTUATION_PCT);

            _logger.LogInformation("{service} took {duration} - will run again in {delay} ms.", nameof(BlogCacheProcessingService), stopwatch.Elapsed, jitteredDelay);

            await Task.Delay(jitteredDelay, stoppingToken);
        }
    }

    async Task UpdateBlogCache(CancellationToken stoppingToken)
    {
        if(stoppingToken.IsCancellationRequested)
        {
            return;
        }

        if(await _cache.GetStatusAsync() != CacheStatus.InitializationSucceeded)
        {
            await _cache.SetStatusAsync(CacheStatus.Initializing);
        }

        var dbBlogs = await _repo.GetBlogsAsync();
        var cacheBlogs = await _cache.GetBlogsAsync();
        var updatedBlogs = dbBlogs.Except(cacheBlogs.Item ?? new List<Blog>());

        if(updatedBlogs.Any())
        {
            await _cache.AddBlogsAsync(updatedBlogs);

            _logger.LogInformation("{service} updated {count} blog(s)", nameof(BlogCacheProcessingService), updatedBlogs.Count());
        }

        await UpdatePostCache(dbBlogs, stoppingToken);

        if(stoppingToken.IsCancellationRequested)
        {
            return;
        }

        await _cache.SetStatusAsync(CacheStatus.InitializationSucceeded);
    }

    async Task UpdatePostCache(IEnumerable<Blog> blogs, CancellationToken stoppingToken)
    {
        foreach(var blog in blogs)
        {
            if(stoppingToken.IsCancellationRequested)
            {
                return;
            }

            await UpdatePostCache(blog);
        }
    }

    async Task UpdatePostCache(Blog blog)
    {
        var dbPosts = await _repo.GetAllPostsAsync(blog.Id);
        var cachePosts = await _cache.GetPostsAsync(blog.Id, null);
        var updatedPosts = dbPosts.Except(cachePosts.Item ?? new List<Post>());

        if(updatedPosts.Any())
        {
            await _cache.AddPostsAsync(updatedPosts);

            _logger.LogInformation("{service} updated {count} posts", nameof(BlogCacheProcessingService), updatedPosts.Count());
        }
    }
}
