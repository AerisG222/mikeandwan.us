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
        _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
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

    public Task<GpsDetail?> GetGpsDetailAsync(int videoId, string[] roles)
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

    public Task SetGpsOverrideAsync(int videoId, GpsCoordinate gps, string username)
    {
        return _repo.SetGpsOverrideAsync(videoId, gps, username);
    }

    public async Task SetCategoryTeaserAsync(short categoryId, int videoId)
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

    public Task ClearCacheAsync()
    {
        return Task.CompletedTask;
    }

    public Task<IEnumerable<CategoryAndRoles>> GetCategoriesAndRolesAsync()
    {
        return _repo.GetCategoriesAndRolesAsync();
    }
}
