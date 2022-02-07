using Maw.Cache.Abstractions;
using Maw.Data.Abstractions;

namespace Maw.Cache.Initializer;

public class VideoCacheWorker
    : BackgroundService
{
    readonly IVideoRepository _repo;
    readonly IVideoCache _cache;
    readonly ILogger _logger;

    public VideoCacheWorker(
        IVideoRepository repo,
        IVideoCache cache,
        ILogger<VideoCacheWorker> logger)
    {
        _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("VideoCacheWorker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(1000, stoppingToken);
        }
    }
}
