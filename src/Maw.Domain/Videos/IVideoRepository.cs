using System.Collections.Generic;
using System.Threading.Tasks;


namespace Maw.Domain.Videos
{
    public interface IVideoRepository
    {
        Task<IEnumerable<short>> GetYearsAsync(bool allowPrivate);
        Task<IEnumerable<Category>> GetAllCategoriesAsync(bool allowPrivate);
        Task<IEnumerable<Category>> GetCategoriesAsync(short year, bool allowPrivate);
        Task<IEnumerable<Video>> GetVideosInCategoryAsync(short categoryId, bool allowPrivate);
        Task<Video> GetVideoAsync(short id, bool allowPrivate);
        Task<Category> GetCategoryAsync(short categoryId, bool allowPrivate);
        Task<IEnumerable<Comment>> GetCommentsAsync(short videoId);
        Task<Rating> GetRatingsAsync(short videoId, string username);
        Task<GpsDetail> GetGpsDetailAsync(int videoId);
        Task<int> InsertCommentAsync(short videoId, string username, string comment);
        Task<float?> SaveRatingAsync(short videoId, string username, short rating);
        Task<float?> RemoveRatingAsync(short videoId, string username);
        Task SetGpsOverrideAsync(int videoId, GpsCoordinate gps, string username);
    }
}
