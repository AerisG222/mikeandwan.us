using Maw.Domain.Models;
using Maw.Domain.Models.Videos;

namespace Maw.Cache.Abstractions;

public interface IVideoCache
    : IBaseCache
{
    Task<CacheResult<IEnumerable<short>>> GetYearsAsync(string[] roles);

    Task<CacheResult<IEnumerable<Category>>> GetCategoriesAsync(string[] roles);
    Task<CacheResult<IEnumerable<Category>>> GetCategoriesAsync(string[] roles, short year);
    Task<CacheResult<Category>> GetCategoryAsync(string[] roles, short categoryId);
    Task AddCategoriesAsync(IEnumerable<SecuredResource<Category>> securedCategories);
    Task AddCategoryAsync(SecuredResource<Category> securedCategory);

    Task<CacheResult<IEnumerable<Video>>> GetVideosAsync(string[] roles, short categoryId);
    Task<CacheResult<Video>> GetVideoAsync(string[] roles, short videoId);
    Task AddVideosAsync(IEnumerable<SecuredResource<Video>> securedVideos);
    Task AddVideoAsync(SecuredResource<Video> securedVideo);
}
