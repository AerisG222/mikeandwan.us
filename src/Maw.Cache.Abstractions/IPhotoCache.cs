using Maw.Domain.Models;
using Maw.Domain.Models.Photos;

namespace Maw.Cache.Abstractions;

public interface IPhotoCache
{
    Task<IEnumerable<short>> GetYearsAsync(string[] roles);

    Task<IEnumerable<Category>> GetCategoriesAsync(string[] roles);
    Task<IEnumerable<Category>> GetCategoriesAsync(string[] roles, short year);
    Task<IEnumerable<Category>> GetRecentCategoriesAsync(string[] roles, short sinceId);
    Task<Category?> GetCategoryAsync(string[] roles, short categoryId);
    Task AddCategoriesAsync(IEnumerable<SecuredResource<Category>> securedCategories);
    Task AddCategoryAsync(SecuredResource<Category> securedCategory);

    Task<IEnumerable<Photo>> GetPhotosAsync(string[] roles, short categoryId);
    Task<IEnumerable<Photo>> GetRandomPhotosAsync(string[] roles, short count);
    Task<Photo?> GetPhotoAsync(string[] roles, int photoId);
    Task AddPhotosAsync(IEnumerable<Photo> photos);
    Task AddPhotoAsync(Photo photo);
}
