using System.Collections.Generic;
using System.Threading.Tasks;


namespace Maw.Domain.Photos
{
    public interface IPhotoRepository
    {
        Task<PhotoAndCategory> GetRandomPhotoAsync(bool allowPrivate);
        Task<List<short>> GetYearsAsync();
        Task<List<Category>> GetCategoriesForYearAsync(short year, bool allowPrivate);
        Task<short> GetCategoryCountAsync(bool allowPrivate);
        Task<List<Category>> GetRecentCategoriesAsync(short sinceId, bool allowPrivate);
        Task<List<Photo>> GetPhotosForCategoryAsync(short categoryId, bool allowPrivate);
        Task<List<Photo>> GetPhotosByCommentDateAsync(bool newestFirst, bool allowPrivate);
        Task<List<Photo>> GetPhotosByUserCommentDateAsync(string username, bool greatestFirst, bool allowPrivate);
        Task<List<Photo>> GetPhotosByCommentCountAsync(bool greatestFirst, bool allowPrivate);
        Task<List<Photo>> GetPhotosByAverageUserRatingAsync(bool highestFirst, bool allowPrivate);
        Task<List<Photo>> GetPhotosByUserRatingAsync(string username, bool highestFirst, bool allowPrivate);
        Task<Category> GetCategoryAsync(short categoryId, bool allowPrivate);
        Task<Photo> GetPhotoAsync(int photoId, bool allowPrivate);
        Task<Detail> GetDetailForPhotoAsync(int photoId, bool allowPrivate);
        Task<List<GpsData>> GetGpsDataForCategoryAsync(short categoryId, bool allowPrivate);
        Task<List<Comment>> GetCommentsForPhotoAsync(int photoId);
        Task<Rating> GetRatingsAsync(int photoId, string username);
        Task<int> InsertPhotoCommentAsync(int photoId, string username, string comment);
        Task<float?> SavePhotoRatingAsync(int photoId, string username, byte rating);
        Task<float?> RemovePhotoRatingAsync(int photoId, string username);
        
        Task<List<PhotoAndCategory>> GetPhotosAndCategoriesByCommentDateAsync(bool newestFirst, bool allowPrivate);
        Task<List<PhotoAndCategory>> GetPhotosAndCategoriesByUserCommentDateAsync(string username, bool greatestFirst, bool allowPrivate);
        Task<List<PhotoAndCategory>> GetPhotosAndCategoriesByCommentCountAsync(bool greatestFirst, bool allowPrivate);
        Task<List<PhotoAndCategory>> GetPhotosAndCategoriesByAverageUserRatingAsync(bool highestFirst, bool allowPrivate);
        Task<List<PhotoAndCategory>> GetPhotosAndCategoriesByUserRatingAsync(string username, bool highestFirst, bool allowPrivate);
    }
}
