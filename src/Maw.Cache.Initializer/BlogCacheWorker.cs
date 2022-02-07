using Maw.Data.Abstractions;
using Maw.Cache.Abstractions;

namespace Maw.Cache.Initializer;

public class BlogCacheWorker
    : BackgroundService
{
    readonly IBlogRepository _repo;
    readonly IBlogCache _cache;
    readonly ILogger _logger;

    public BlogCacheWorker(
        IBlogRepository repo,
        IBlogCache cache,
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
            _logger.LogInformation("BlogCacheWorker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(1000, stoppingToken);
        }
    }
}
