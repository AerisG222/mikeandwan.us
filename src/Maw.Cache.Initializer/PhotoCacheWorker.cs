using Maw.Cache.Photos;
using Maw.Data;

namespace Maw.Cache.Initializer;

public class PhotoCacheWorker
    : BackgroundService
{
    readonly PhotoRepository _repo;
    readonly PhotoCache _cache;
    readonly ILogger _logger;

    public PhotoCacheWorker(
        PhotoRepository repo,
        PhotoCache cache,
        ILogger<PhotoCacheWorker> logger)
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
