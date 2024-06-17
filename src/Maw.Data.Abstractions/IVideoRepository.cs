using Maw.Domain.Models;
using Maw.Domain.Models.Videos;

namespace Maw.Data.Abstractions;

public interface IVideoRepository
{
    Task<IEnumerable<short>> GetYearsAsync(string[] roles);
    Task<IEnumerable<Category>> GetAllCategoriesAsync(string[] roles);
    Task<IEnumerable<Category>> GetCategoriesAsync(short year, string[] roles);
    Task<IEnumerable<Category>> GetRecentCategoriesAsync(short sinceId, string[] roles);
    Task<IEnumerable<Video>> GetVideosInCategoryAsync(short categoryId, string[] roles);
    Task<Video?> GetVideoAsync(short id, string[] roles);
    Task<Category?> GetCategoryAsync(short categoryId, string[]? roles);
    Task<Category?> GetCategoryForVideoAsync(short videoId, string[]? roles);
    Task<IEnumerable<Comment>> GetCommentsAsync(short videoId, string[] roles);
    Task<Rating?> GetRatingsAsync(short videoId, string username, string[] roles);
    Task<GpsDetail?> GetGpsDetailAsync(short videoId, string[] roles);
    Task InsertCommentAsync(short videoId, string username, string comment, string[] roles);
    Task<float?> SaveRatingAsync(short videoId, string username, short rating, string[] roles);
    Task<float?> RemoveRatingAsync(short videoId, string username, string[] roles);

    // admin functions
    Task SetGpsOverrideAsync(short videoId, GpsCoordinate gps, string username);
    Task<long> SetCategoryTeaserAsync(short categoryId, short videoId);
    Task<IEnumerable<CategoryAndRoles>> GetCategoriesAndRolesAsync();
}
