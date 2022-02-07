using Maw.Domain.Models;
using Maw.Domain.Models.Photos;

namespace Maw.Cache.Abstractions;

public interface IPhotoCache
{
    Task<CacheResult<IEnumerable<short>>> GetYearsAsync(string[] roles);

    Task<CacheResult<IEnumerable<Category>>> GetCategoriesAsync(string[] roles);
    Task<CacheResult<IEnumerable<Category>>> GetCategoriesAsync(string[] roles, short year);
    Task<CacheResult<IEnumerable<Category>>> GetRecentCategoriesAsync(string[] roles, short sinceId);
    Task<CacheResult<Category>> GetCategoryAsync(string[] roles, short categoryId);
    Task AddCategoriesAsync(IEnumerable<SecuredResource<Category>> securedCategories);
    Task AddCategoryAsync(SecuredResource<Category> securedCategory);

    Task<CacheResult<IEnumerable<Photo>>> GetPhotosAsync(string[] roles, short categoryId);
    Task<CacheResult<IEnumerable<Photo>>> GetRandomPhotosAsync(string[] roles, short count);
    Task<CacheResult<Photo>> GetPhotoAsync(string[] roles, int photoId);
    Task<Detail?> GetPhotoDetailsAsync(string[] roles, int photoId);
    Task AddPhotosAsync(IEnumerable<SecuredResource<Photo>> securedPhotos);
    Task AddPhotoAsync(SecuredResource<Photo> securedPhoto);
    Task AddPhotoDetailsAsync(int photoId, Detail detail);
}
