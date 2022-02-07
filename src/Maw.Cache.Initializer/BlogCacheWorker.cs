using Maw.Data;
using Maw.Cache.Blogs;

namespace Maw.Cache.Initializer;

public class BlogCacheWorker
    : BackgroundService
{
    readonly BlogRepository _repo;
    readonly BlogCache _cache;
    readonly ILogger _logger;

    public BlogCacheWorker(
        BlogRepository repo,
        BlogCache cache,
        ILogger<BlogCacheWorker> logger)
    {
        _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(1000, stoppingToken);
        }
    }
}
