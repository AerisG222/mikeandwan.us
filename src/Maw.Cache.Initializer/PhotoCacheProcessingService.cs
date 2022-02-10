using Maw.Cache.Abstractions;
using Maw.Data.Abstractions;

namespace Maw.Cache.Initializer;

public class PhotoCacheProcessingService
    : IScopedProcessingService
{
    const int BASE_DELAY = 5_000;
    const float DELAY_FLUCTUATION_PCT = 0.25f;

    readonly IPhotoRepository _repo;
    readonly IPhotoCache _cache;
    readonly IDelayCalculator _delay;
    readonly ILogger _logger;

    public PhotoCacheProcessingService(
        IPhotoRepository repo,
        IPhotoCache cache,
        IDelayCalculator delayCalculator,
        ILogger<PhotoCacheProcessingService> logger)
    {
        _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _delay = delayCalculator ?? throw new ArgumentNullException(nameof(delayCalculator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task DoWorkAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("{service} running at: {time}", nameof(PhotoCacheProcessingService), DateTimeOffset.Now);

            var jitteredDelay = _delay.CalculateRandomizedDelay(BASE_DELAY, DELAY_FLUCTUATION_PCT);

            _logger.LogInformation("{service} will run again in {delay} ms.", nameof(PhotoCacheProcessingService), jitteredDelay);

            await Task.Delay(jitteredDelay, stoppingToken);
        }
    }
}
