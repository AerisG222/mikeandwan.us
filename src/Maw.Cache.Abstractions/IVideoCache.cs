using Maw.Domain.Models;
using Maw.Domain.Models.Videos;

namespace Maw.Cache.Abstractions;

public interface IVideoCache
{
    Task<IEnumerable<short>> GetYearsAsync(string[] roles);

    Task<IEnumerable<Category>> GetCategoriesAsync(string[] roles);
    Task<IEnumerable<Category>> GetCategoriesAsync(string[] roles, short year);
    Task<Category?> GetCategoryAsync(string[] roles, short categoryId);
    Task AddCategoriesAsync(IEnumerable<SecuredResource<Category>> securedCategories);
    Task AddCategoryAsync(SecuredResource<Category> securedCategory);

    Task<IEnumerable<Video>> GetVideosAsync(string[] roles, short categoryId);
    Task<Video?> GetVideoAsync(string[] roles, int videoId);
    Task AddVideosAsync(IEnumerable<SecuredResource<Video>> securedVideos);
    Task AddVideoAsync(SecuredResource<Video> securedVideo);
}
