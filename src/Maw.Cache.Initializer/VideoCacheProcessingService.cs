using System.Diagnostics;
using Maw.Cache.Abstractions;
using Maw.Data.Abstractions;
using Maw.Domain.Models;
using Maw.Domain.Models.Videos;

namespace Maw.Cache.Initializer;

public class VideoCacheProcessingService
    : IScopedProcessingService
{
    const int BASE_DELAY = 60_000;
    const float DELAY_FLUCTUATION_PCT = 0.25f;

    readonly IVideoRepository _repo;
    readonly IVideoCache _cache;
    readonly IDelayCalculator _delay;
    readonly ILogger _logger;

    public VideoCacheProcessingService(
        IVideoRepository repo,
        IVideoCache cache,
        IDelayCalculator delayCalculator,
        ILogger<VideoCacheProcessingService> logger)
    {
        _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _delay = delayCalculator ?? throw new ArgumentNullException(nameof(delayCalculator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));;
    }

    public async Task DoWorkAsync(CancellationToken stoppingToken)
    {
        var stopwatch = new Stopwatch();

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("{service} running at: {time}", nameof(VideoCacheProcessingService), DateTimeOffset.Now);

            stopwatch.Restart();
            await UpdateCategoryCache(stoppingToken);
            stopwatch.Stop();

            var jitteredDelay = _delay.CalculateRandomizedDelay(BASE_DELAY, DELAY_FLUCTUATION_PCT);

            _logger.LogInformation("{service} took {duration} - will run again in {delay} ms.", nameof(VideoCacheProcessingService), stopwatch.Elapsed, jitteredDelay);

            await Task.Delay(jitteredDelay, stoppingToken);
        }
    }

    // TODO: support removing categories and permissions from cache
    async Task UpdateCategoryCache(CancellationToken stoppingToken)
    {
        if(stoppingToken.IsCancellationRequested)
        {
            return;
        }

        if(await _cache.GetStatusAsync() != CacheStatus.InitializationSucceeded)
        {
            await _cache.SetStatusAsync(CacheStatus.Initializing);
        }

        var dbCategoriesAndRoles = await _repo.GetCategoriesAndRolesAsync();
        var allRoles = dbCategoriesAndRoles.SelectMany(x => x.Roles).Distinct().ToArray();
        var dbCategories = await _repo.GetAllCategoriesAsync(allRoles);
        var cacheCategories = await _cache.GetCategoriesAsync(allRoles);
        var updatedCategories = dbCategories.Except(cacheCategories.Item ?? new List<Category>());

        if(updatedCategories.Count() > 0)
        {
            var securedCategories = updatedCategories
                .Select(category => new SecuredResource<Category>(
                    category,
                    dbCategoriesAndRoles
                        .First(x => x.Id == category.Id)
                        .Roles
                ));

            await _cache.AddCategoriesAsync(securedCategories);

            _logger.LogInformation("{service} updated {count} video categories", nameof(VideoCacheProcessingService), updatedCategories.Count());
        }

        await UpdateVideoCache(dbCategories, dbCategoriesAndRoles, stoppingToken);

        if(stoppingToken.IsCancellationRequested)
        {
            return;
        }

        await _cache.SetStatusAsync(CacheStatus.InitializationSucceeded);
    }

    async Task UpdateVideoCache(
        IEnumerable<Category> categories,
        IEnumerable<CategoryAndRoles> categoriesAndRoles,
        CancellationToken stoppingToken)
    {
        foreach(var category in categories)
        {
            if(stoppingToken.IsCancellationRequested)
            {
                return;
            }

            await UpdateVideoCache(category, categoriesAndRoles.First(x => x.Id == category.Id).Roles);
        }
    }

    async Task UpdateVideoCache(Category category, string[] allRoles)
    {
        var dbVideos = await _repo.GetVideosInCategoryAsync(category.Id, allRoles);
        var cacheVideos = await _cache.GetVideosAsync(allRoles, category.Id);
        var updatedVideos = dbVideos.Except(cacheVideos.Item ?? new List<Video>());

        if(updatedVideos.Count() > 0)
        {
            var securedVideos = updatedVideos.Select(video => new SecuredResource<Video>(
                video,
                allRoles
            ));

            await _cache.AddVideosAsync(securedVideos);
        }
    }
}
