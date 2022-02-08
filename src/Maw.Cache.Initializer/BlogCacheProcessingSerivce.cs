using Maw.Data.Abstractions;
using Maw.Cache.Abstractions;

namespace Maw.Cache.Initializer;

internal class BlogCacheProcessingService
    : IScopedProcessingService
{
    readonly IBlogRepository _repo;
    readonly IBlogCache _cache;
    readonly ILogger _logger;

    public BlogCacheProcessingService(
        IBlogRepository repo,
        IBlogCache cache,
        ILogger<BlogCacheProcessingService> logger)
    {
        _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task DoWorkAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("{service} running at: {time}", nameof(BlogCacheProcessingService), DateTimeOffset.Now);

            await Task.Delay(5000, stoppingToken);
        }
    }
}
