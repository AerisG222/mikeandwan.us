using Microsoft.Extensions.Logging;
using Maw.Cache.Abstractions;
using Maw.Data.Abstractions;
using Maw.Domain.Models;
using Maw.Domain.Models.Videos;

namespace Maw.Domain.Videos;

public class VideoService
    : BaseService, IVideoService
{
    readonly IVideoRepository _repo;
    readonly IVideoCache _cache;

    public VideoService(
        IVideoRepository repo,
        IVideoCache cache,
        ILogger<VideoService> log)
        : base(log)
    {
        ArgumentNullException.ThrowIfNull(repo);
        ArgumentNullException.ThrowIfNull(cache);

        _repo = repo;
        _cache = cache;
    }

    public async Task<IEnumerable<short>> GetYearsAsync(string[] roles)
    {
        var years = await GetCachedValueAsync(
            () => _cache.GetYearsAsync(roles),
            () => _repo.GetYearsAsync(roles)
        );

        return years ?? new List<short>();
    }

    public async Task<IEnumerable<Category>> GetAllCategoriesAsync(string[] roles)
    {
        var categories = await GetCachedValueAsync(
            () => _cache.GetCategoriesAsync(roles),
            () => _repo.GetAllCategoriesAsync(roles)
        );

        return categories ?? new List<Category>();
    }

    public async Task<IEnumerable<Category>> GetCategoriesAsync(short year, string[] roles)
    {
        var categories = await GetCachedValueAsync(
            () => _cache.GetCategoriesAsync(roles, year),
            () => _repo.GetCategoriesAsync(year, roles)
        );

        return categories ?? new List<Category>();
    }

    public async Task<IEnumerable<Category>> GetRecentCategoriesAsync(short sinceId, string[] roles)
    {
        return await GetCachedValueAsync(
            () => _cache.GetRecentCategoriesAsync(roles, sinceId),
            () => _repo.GetRecentCategoriesAsync(sinceId, roles)
        ) ;
    }

    public async Task<IEnumerable<Video>> GetVideosInCategoryAsync(short categoryId, string[] roles)
    {
        var videos = await GetCachedValueAsync(
            () => _cache.GetVideosAsync(roles, categoryId),
            () => _repo.GetVideosInCategoryAsync(categoryId, roles)
        );

        return videos ?? new List<Video>();
    }

    public async Task<Video?> GetVideoAsync(short id, string[] roles)
    {
        return await GetNullableCachedValueAsync(
            () => _cache.GetVideoAsync(roles, id),
            () => _repo.GetVideoAsync(id, roles)
        );
    }

    public async Task<Category?> GetCategoryAsync(short categoryId, string[] roles)
    {
        return await GetNullableCachedValueAsync(
            () => _cache.GetCategoryAsync(roles, categoryId),
            () => _repo.GetCategoryAsync(categoryId, roles)
        );
    }

    public Task<IEnumerable<Comment>> GetCommentsAsync(short videoId, string[] roles)
    {
        return _repo.GetCommentsAsync(videoId, roles);
    }

    public Task<GpsDetail?> GetGpsDetailAsync(short videoId, string[] roles)
    {
        return _repo.GetGpsDetailAsync(videoId, roles);
    }

    public Task<Rating?> GetRatingsAsync(short videoId, string username, string[] roles)
    {
        return _repo.GetRatingsAsync(videoId, username, roles);
    }

    public Task InsertCommentAsync(short videoId, string username, string comment, string[] roles)
    {
        return _repo.InsertCommentAsync(videoId, username, comment, roles);
    }

    public Task<float?> SaveRatingAsync(short videoId, string username, short rating, string[] roles)
    {
        return _repo.SaveRatingAsync(videoId, username, rating, roles);
    }

    public Task<float?> RemoveRatingAsync(short videoId, string username, string[] roles)
    {
        return _repo.RemoveRatingAsync(videoId, username, roles);
    }

    public async Task SetGpsOverrideAsync(short videoId, GpsCoordinate gps, string username, string[] roles)
    {
        await _repo.SetGpsOverrideAsync(videoId, gps, username);

        var category = await _repo.GetCategoryForVideoAsync(videoId, roles);

        if(category != null && !category.IsMissingGpsData) {
            await _cache.AddCategoryAsync(new SecuredResource<Category>(category, Array.Empty<string>()));
        }
    }

    public async Task SetCategoryTeaserAsync(short categoryId, short videoId)
    {
        var count = await _repo.SetCategoryTeaserAsync(categoryId, videoId);

        if (count != 1)
        {
            throw new ApplicationException("Did not update category teaser!");
        }

        var cat = await _repo.GetCategoryAsync(categoryId, null);

        if(cat != null)
        {
            await _cache.AddCategoryAsync(new SecuredResource<Category>(cat, Array.Empty<string>()));
        }
    }

    public Task<IEnumerable<CategoryAndRoles>> GetCategoriesAndRolesAsync()
    {
        return _repo.GetCategoriesAndRolesAsync();
    }
}
