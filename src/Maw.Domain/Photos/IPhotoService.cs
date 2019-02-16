using System.Collections.Generic;
using System.Threading.Tasks;


namespace Maw.Domain.Photos
{
    public interface IPhotoService
    {
        Task<Photo> GetRandomPhotoAsync(bool allowPrivate);
        Task<IEnumerable<Photo>> GetRandomPhotosAsync(byte count, bool allowPrivate);
        Task<IEnumerable<short>> GetYearsAsync();
        Task<IEnumerable<Category>> GetAllCategoriesAsync(bool allowPrivate);
        Task<IEnumerable<Category>> GetCategoriesForYearAsync(short year, bool allowPrivate);
        Task<short> GetCategoryCountAsync(bool allowPrivate);
        Task<IEnumerable<Category>> GetRecentCategoriesAsync(short sinceId, bool allowPrivate);
        Task<IEnumerable<Photo>> GetPhotosForCategoryAsync(short categoryId, bool allowPrivate);
        Task<IEnumerable<Photo>> GetPhotosByCommentDateAsync(bool newestFirst, bool allowPrivate);
        Task<IEnumerable<Photo>> GetPhotosByUserCommentDateAsync(string username, bool greatestFirst, bool allowPrivate);
        Task<IEnumerable<Photo>> GetPhotosByCommentCountAsync(bool greatestFirst, bool allowPrivate);
        Task<IEnumerable<Photo>> GetPhotosByAverageUserRatingAsync(bool highestFirst, bool allowPrivate);
        Task<IEnumerable<Photo>> GetPhotosByUserRatingAsync(string username, bool highestFirst, bool allowPrivate);
        Task<Category> GetCategoryAsync(short categoryId, bool allowPrivate);
        Task<Photo> GetPhotoAsync(int photoId, bool allowPrivate);
        Task<Detail> GetDetailForPhotoAsync(int photoId, bool allowPrivate);
        Task<IEnumerable<Comment>> GetCommentsForPhotoAsync(int photoId);
        Task<Rating> GetRatingsAsync(int photoId, string username);
        Task<int> InsertPhotoCommentAsync(int photoId, string username, string comment);
        Task<float?> SavePhotoRatingAsync(int photoId, string username, byte rating);
        Task<float?> RemovePhotoRatingAsync(int photoId, string username);
    }
}
