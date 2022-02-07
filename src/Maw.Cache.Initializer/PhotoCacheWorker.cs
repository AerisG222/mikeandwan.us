using Maw.Cache.Abstractions;
using Maw.Data.Abstractions;

namespace Maw.Cache.Initializer;

public class PhotoCacheWorker
    : BackgroundService
{
    readonly IPhotoRepository _repo;
    readonly IPhotoCache _cache;
    readonly ILogger _logger;

    public PhotoCacheWorker(
        IPhotoRepository repo,
        IPhotoCache cache,
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
            _logger.LogInformation("PhotoCacheWorker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(1000, stoppingToken);
        }
    }
}
