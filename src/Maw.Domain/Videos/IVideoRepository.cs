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
    }
}
