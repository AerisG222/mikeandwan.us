using System.Collections.Generic;
using System.Threading.Tasks;


namespace Maw.Domain.Videos
{
    public interface IVideoService
    {
        Task<IEnumerable<short>> GetYearsAsync(bool allowPrivate);
        Task<IEnumerable<Category>> GetAllCategoriesAsync(bool allowPrivate);
        Task<IEnumerable<Category>> GetCategoriesAsync(short year, bool allowPrivate);
        Task<IEnumerable<Video>> GetVideosInCategoryAsync(short categoryId, bool allowPrivate);
        Task<Video> GetVideoAsync(short id, bool allowPrivate);
        Task<Category> GetCategoryAsync(short categoryId, bool allowPrivate);
        Task<IEnumerable<Comment>> GetCommentsAsync(int videoId);
        Task<Rating> GetRatingsAsync(int videoId, string username);
        Task<int> InsertCommentAsync(int videoId, string username, string comment);
        Task<float?> SaveRatingAsync(int videoId, string username, byte rating);
        Task<float?> RemoveRatingAsync(int videoId, string username);
    }
}
