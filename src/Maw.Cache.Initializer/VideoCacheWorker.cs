using Maw.Cache.Videos;
using Maw.Data;

namespace Maw.Cache.Initializer;

public class VideoCacheWorker
    : BackgroundService
{
    readonly VideoRepository _repo;
    readonly VideoCache _cache;
    readonly ILogger _logger;

    public VideoCacheWorker(
        VideoRepository repo,
        VideoCache cache,
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
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(1000, stoppingToken);
        }
    }
}
