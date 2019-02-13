using System.Collections.Generic;
using System.Threading.Tasks;
using Maw.Domain.Photos.ThreeD;


namespace Maw.Domain.Photos
{
    public interface IPhotoRepository
    {
        Task<PhotoAndCategory> GetRandomPhotoAsync(bool allowPrivate);
        Task<IEnumerable<short>> GetYearsAsync();
        Task<IEnumerable<Category>> GetAllCategoriesAsync(bool allowPrivate);
        Task<IEnumerable<Category>> GetCategoriesForYearAsync(short year, bool allowPrivate);
        Task<short> GetCategoryCountAsync(bool allowPrivate);
        Task<IEnumerable<Category>> GetRecentCategoriesAsync(short sinceId, bool allowPrivate);
        Task<IEnumerable<Photo>> GetPhotosForCategoryAsync(short categoryId, bool allowPrivate);
        Task<Category> GetCategoryAsync(short categoryId, bool allowPrivate);
        Task<Photo> GetPhotoAsync(int photoId, bool allowPrivate);
        Task<Detail> GetDetailForPhotoAsync(int photoId, bool allowPrivate);
        Task<IEnumerable<Comment>> GetCommentsForPhotoAsync(int photoId);
        Task<Rating> GetRatingsAsync(int photoId, string username);
        Task<int> InsertPhotoCommentAsync(int photoId, string username, string comment);
        Task<float?> SavePhotoRatingAsync(int photoId, string username, byte rating);
        Task<float?> RemovePhotoRatingAsync(int photoId, string username);

        Task<IEnumerable<PhotoAndCategory>> GetPhotosAndCategoriesByCommentDateAsync(bool newestFirst, bool allowPrivate);
        Task<IEnumerable<PhotoAndCategory>> GetPhotosAndCategoriesByUserCommentDateAsync(string username, bool greatestFirst, bool allowPrivate);
        Task<IEnumerable<PhotoAndCategory>> GetPhotosAndCategoriesByCommentCountAsync(bool greatestFirst, bool allowPrivate);
        Task<IEnumerable<PhotoAndCategory>> GetPhotosAndCategoriesByAverageUserRatingAsync(bool highestFirst, bool allowPrivate);
        Task<IEnumerable<PhotoAndCategory>> GetPhotosAndCategoriesByUserRatingAsync(string username, bool highestFirst, bool allowPrivate);

        Task<IEnumerable<CategoryPhotoCount>> GetStats(bool allowPrivate);

        Task<IEnumerable<Category3D>> GetAllCategories3D();
        Task<IEnumerable<Photo3D>> GetPhotos3D(int categoryId);
    }
}
