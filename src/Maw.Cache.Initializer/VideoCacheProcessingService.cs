using Maw.Cache.Abstractions;
using Maw.Data.Abstractions;

namespace Maw.Cache.Initializer;

public class VideoCacheProcessingService
    : IScopedProcessingService
{
    readonly IVideoRepository _repo;
    readonly IVideoCache _cache;
    readonly ILogger _logger;

    public VideoCacheProcessingService(
        IVideoRepository repo,
        IVideoCache cache,
        ILogger<VideoCacheProcessingService> logger)
    {
        _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));;
    }

    public async Task DoWorkAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("{service} running at: {time}", nameof(VideoCacheProcessingService), DateTimeOffset.Now);

            await Task.Delay(5000, stoppingToken);
        }
    }
}
