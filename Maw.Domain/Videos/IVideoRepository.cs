using System.Collections.Generic;
using System.Threading.Tasks;


namespace Maw.Domain.Videos
{
    public interface IVideoRepository
    {
        Task<List<short>> GetYearsAsync(bool allowPrivate);
        Task<List<Category>> GetCategoriesAsync(short year, bool allowPrivate);
        Task<List<Video>> GetVideosInCategoryAsync(short categoryId, bool allowPrivate);
        Task<Video> GetVideoAsync(short id, bool allowPrivate);
        Task<Category> GetCategoryAsync(short categoryId, bool allowPrivate);
    }
}
