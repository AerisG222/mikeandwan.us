using System.Collections.Generic;
using System.Threading.Tasks;

namespace Maw.Domain.Photos
{
    public interface IPhotoService
    {
        Task<Photo> GetRandomAsync(string[] roles);
        Task<IEnumerable<Photo>> GetRandomAsync(byte count, string[] roles);
        Task<IEnumerable<short>> GetYearsAsync(string[] roles);
        Task<IEnumerable<Category>> GetAllCategoriesAsync(string[] roles);
        Task<IEnumerable<Category>> GetCategoriesForYearAsync(short year, string[] roles);
        Task<IEnumerable<Category>> GetRecentCategoriesAsync(short sinceId, string[] roles);
        Task<IEnumerable<Photo>> GetPhotosForCategoryAsync(short categoryId, string[] roles);
        Task<Category> GetCategoryAsync(short categoryId, string[] roles);
        Task<Photo> GetPhotoAsync(int photoId, string[] roles);
        Task<Detail> GetDetailAsync(int photoId, string[] roles);
        Task<IEnumerable<Comment>> GetCommentsAsync(int photoId, string[] roles);
        Task<Rating> GetRatingsAsync(int photoId, string username, string[] roles);
        Task<GpsDetail> GetGpsDetailAsync(int photoId, string[] roles);
        Task<int> InsertCommentAsync(int photoId, string username, string comment, string[] roles);
        Task<float?> SaveRatingAsync(int photoId, string username, short rating, string[] roles);
        Task<float?> RemoveRatingAsync(int photoId, string username, string[] roles);

        // admin functions
        Task SetGpsOverrideAsync(int photoId, GpsCoordinate gps, string username);
        Task SetCategoryTeaserAsync(short categoryId, int photoId);
        Task ClearCacheAsync();
    }
}
