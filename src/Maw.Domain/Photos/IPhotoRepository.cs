using System.Collections.Generic;
using System.Threading.Tasks;


namespace Maw.Domain.Photos
{
    public interface IPhotoRepository
    {
        Task<Photo> GetRandomAsync(bool allowPrivate);
        Task<IEnumerable<Photo>> GetRandomAsync(byte count, bool allowPrivate);
        Task<IEnumerable<short>> GetYearsAsync();
        Task<IEnumerable<Category>> GetAllCategoriesAsync(bool allowPrivate);
        Task<IEnumerable<Category>> GetCategoriesForYearAsync(short year, bool allowPrivate);
        Task<IEnumerable<Category>> GetRecentCategoriesAsync(short sinceId, bool allowPrivate);
        Task<IEnumerable<Photo>> GetPhotosForCategoryAsync(short categoryId, bool allowPrivate);
        Task<Category> GetCategoryAsync(short categoryId, bool allowPrivate);
        Task<Photo> GetPhotoAsync(int photoId, bool allowPrivate);
        Task<Detail> GetDetailAsync(int photoId, bool allowPrivate);
        Task<IEnumerable<Comment>> GetCommentsAsync(int photoId);
        Task<Rating> GetRatingsAsync(int photoId, string username);
        Task<int> InsertCommentAsync(int photoId, string username, string comment);
        Task<float?> SaveRatingAsync(int photoId, string username, short rating);
        Task<float?> RemoveRatingAsync(int photoId, string username);
    }
}
