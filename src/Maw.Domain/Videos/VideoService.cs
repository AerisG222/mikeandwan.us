using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Maw.Data.Abstractions;
using Maw.Domain.Models;
using Maw.Domain.Models.Videos;

namespace Maw.Domain.Videos;

public class VideoService
    : BaseService, IVideoService
{
    readonly IVideoRepository _repo;

    public VideoService(
        IVideoRepository videoRepository,
        ILogger<VideoService> log,
        IDistributedCache cache)
        : base("videos", log, cache)
    {
        _repo = videoRepository ?? throw new ArgumentNullException(nameof(videoRepository));
    }

    public async Task<IEnumerable<short>> GetYearsAsync(string[] roles)
    {
        var key = $"{nameof(GetYearsAsync)}_{GetRoleCacheKeyComponent(roles)}";
        var years = await GetCachedValueAsync(key, () => _repo.GetYearsAsync(roles));

        return years ?? new List<short>();
    }

    public async Task<IEnumerable<Category>> GetAllCategoriesAsync(string[] roles)
    {
        var key = $"{nameof(GetAllCategoriesAsync)}_{GetRoleCacheKeyComponent(roles)}";
        var categories = await GetCachedValueAsync(key, () => _repo.GetAllCategoriesAsync(roles));

        return categories ?? new List<Category>();
    }

    public async Task<IEnumerable<Category>> GetCategoriesAsync(short year, string[] roles)
    {
        var key = $"{nameof(GetCategoriesAsync)}_{year}_{GetRoleCacheKeyComponent(roles)}";
        var categories = await GetCachedValueAsync(key, () => _repo.GetCategoriesAsync(year, roles));

        return categories ?? new List<Category>();
    }

    public async Task<IEnumerable<Video>> GetVideosInCategoryAsync(short categoryId, string[] roles)
    {
        var key = $"{nameof(GetVideosInCategoryAsync)}_{categoryId}_{GetRoleCacheKeyComponent(roles)}";
        var videos = await GetCachedValueAsync(key, () => _repo.GetVideosInCategoryAsync(categoryId, roles), TimeSpan.FromHours(2));

        return videos ?? new List<Video>();
    }

    public Task<Video?> GetVideoAsync(short id, string[] roles)
    {
        var key = $"{nameof(GetVideoAsync)}_{id}_{GetRoleCacheKeyComponent(roles)}";

        return GetCachedValueAsync(key, () => _repo.GetVideoAsync(id, roles), TimeSpan.FromHours(2));
    }

    public Task<Category?> GetCategoryAsync(short categoryId, string[] roles)
    {
        var key = $"{nameof(GetCategoryAsync)}_{categoryId}_{GetRoleCacheKeyComponent(roles)}";

        return GetCachedValueAsync(key, () => _repo.GetCategoryAsync(categoryId, roles), TimeSpan.FromHours(2));
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

        await ClearCacheAsync();
    }

    public Task ClearCacheAsync()
    {
        return InternalClearCacheAsync();
    }

    static string GetRoleCacheKeyComponent(string[] roles)
    {
        return string.Join("_", roles);
    }
}
