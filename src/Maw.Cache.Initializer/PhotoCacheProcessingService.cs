using System.Diagnostics;
using Maw.Cache.Abstractions;
using Maw.Data.Abstractions;
using Maw.Domain.Models;
using Maw.Domain.Models.Photos;

namespace Maw.Cache.Initializer;

public class PhotoCacheProcessingService
    : IScopedProcessingService
{
    const int BASE_DELAY = 60_000;
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
        var stopwatch = new Stopwatch();

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("{service} running at: {time}", nameof(PhotoCacheProcessingService), DateTimeOffset.Now);

            stopwatch.Restart();
            await UpdateCategoryCache(stoppingToken);
            stopwatch.Stop();

            var jitteredDelay = _delay.CalculateRandomizedDelay(BASE_DELAY, DELAY_FLUCTUATION_PCT);

            _logger.LogInformation("{service} took {duration} - will run again in {delay} ms.", nameof(PhotoCacheProcessingService), stopwatch.Elapsed, jitteredDelay);

            await Task.Delay(jitteredDelay, stoppingToken);
        }
    }

    // TODO: support removing categories and permissions from cache
    async Task UpdateCategoryCache(CancellationToken stoppingToken)
    {
        if(stoppingToken.IsCancellationRequested)
        {
            _logger.LogWarning("a");

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

            _logger.LogInformation("{service} updated {count} photo categories", nameof(PhotoCacheProcessingService), updatedCategories.Count());
        }

        await UpdatePhotoCache(dbCategories, dbCategoriesAndRoles, stoppingToken);

        if(stoppingToken.IsCancellationRequested)
        {
            _logger.LogWarning("b");
            return;
        }

        await _cache.SetStatusAsync(CacheStatus.InitializationSucceeded);
    }

    async Task UpdatePhotoCache(
        IEnumerable<Category> categories,
        IEnumerable<CategoryAndRoles> categoriesAndRoles,
        CancellationToken stoppingToken)
    {
        foreach(var category in categories)
        {
            if(stoppingToken.IsCancellationRequested)
            {
                _logger.LogWarning("c");
                return;
            }

            await UpdatePhotoCache(category, categoriesAndRoles.First(x => x.Id == category.Id).Roles);
        }
    }

    async Task UpdatePhotoCache(Category category, string[] allRoles)
    {
        var dbPhotos = await _repo.GetPhotosForCategoryAsync(category.Id, allRoles);
        var cachePhotos = await _cache.GetPhotosAsync(allRoles, category.Id);
        var updatedPhotos = dbPhotos.Except(cachePhotos.Item ?? new List<Photo>());

        if(updatedPhotos.Count() > 0)
        {
            var securedPhotos = updatedPhotos.Select(photo => new SecuredResource<Photo>(
                photo,
                allRoles
            ));

            await _cache.AddPhotosAsync(securedPhotos);
        }
    }
}
