using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Maw.Domain.Photos
{
    public class PhotoService
        : BaseService, IPhotoService
    {
        readonly IPhotoRepository _repo;

        public PhotoService(IPhotoRepository photoRepository,
                            ILogger<PhotoService> log,
                            IDistributedCache cache)
            : base("photos", log, cache)
        {
            _repo = photoRepository ?? throw new ArgumentNullException(nameof(photoRepository));
        }

        public Task<Photo> GetRandomAsync(string[] roles)
        {
            return _repo.GetRandomAsync(roles);
        }

        public Task<IEnumerable<Photo>> GetRandomAsync(byte count, string[] roles)
        {
            return _repo.GetRandomAsync(count, roles);
        }

        public Task<IEnumerable<short>> GetYearsAsync(string[] roles)
        {
            return GetCachedValueAsync(nameof(GetYearsAsync), () => _repo.GetYearsAsync(roles));
        }

        public Task<IEnumerable<Category>> GetAllCategoriesAsync(string[] roles)
        {
            var key = $"{nameof(GetAllCategoriesAsync)}_{GetRoleCacheKeyComponent(roles)}";

            return GetCachedValueAsync(key, () => _repo.GetAllCategoriesAsync(roles));
        }

        public Task<IEnumerable<Category>> GetCategoriesForYearAsync(short year, string[] roles)
        {
            var key = $"{nameof(GetCategoriesForYearAsync)}_{year}_{GetRoleCacheKeyComponent(roles)}";

            return GetCachedValueAsync(key, () => _repo.GetCategoriesForYearAsync(year, roles));
        }

        public Task<IEnumerable<Category>> GetRecentCategoriesAsync(short sinceId, string[] roles)
        {
            return _repo.GetRecentCategoriesAsync(sinceId, roles);
        }

        public Task<IEnumerable<Photo>> GetPhotosForCategoryAsync(short categoryId, string[] roles)
        {
            var key = $"{nameof(GetPhotosForCategoryAsync)}_{categoryId}_{GetRoleCacheKeyComponent(roles)}";

            return GetCachedValueAsync(key, () => _repo.GetPhotosForCategoryAsync(categoryId, roles), TimeSpan.FromHours(2));
        }

        public Task<Category> GetCategoryAsync(short categoryId, string[] roles)
        {
            var key = $"{nameof(GetCategoryAsync)}_{categoryId}_{GetRoleCacheKeyComponent(roles)}";

            return GetCachedValueAsync(key, () => _repo.GetCategoryAsync(categoryId, roles), TimeSpan.FromHours(2));
        }

        public Task<Photo> GetPhotoAsync(int photoId, string[] roles)
        {
            var key = $"{nameof(GetPhotoAsync)}_{photoId}_{GetRoleCacheKeyComponent(roles)}";

            return GetCachedValueAsync(key, () => _repo.GetPhotoAsync(photoId, roles), TimeSpan.FromHours(2));
        }

        public Task<Detail> GetDetailAsync(int photoId, string[] roles)
        {
            var key = $"{nameof(GetDetailAsync)}_{photoId}_{GetRoleCacheKeyComponent(roles)}";

            return GetCachedValueAsync(key, () => _repo.GetDetailAsync(photoId, roles), TimeSpan.FromMinutes(15));
        }

        public Task<IEnumerable<Comment>> GetCommentsAsync(int photoId, string[] roles)
        {
            return _repo.GetCommentsAsync(photoId, roles);
        }

        public Task<Rating> GetRatingsAsync(int photoId, string username, string[] roles)
        {
            return _repo.GetRatingsAsync(photoId, username, roles);
        }

        public Task<GpsDetail> GetGpsDetailAsync(int photoId, string[] roles)
        {
            return _repo.GetGpsDetailAsync(photoId, roles);
        }

        public Task<int> InsertCommentAsync(int photoId, string username, string comment, string[] roles)
        {
            return _repo.InsertCommentAsync(photoId, username, comment, roles);
        }

        public Task<float?> SaveRatingAsync(int photoId, string username, short rating, string[] roles)
        {
            return _repo.SaveRatingAsync(photoId, username, rating, roles);
        }

        public Task<float?> RemoveRatingAsync(int photoId, string username, string[] roles)
        {
            return _repo.RemoveRatingAsync(photoId, username, roles);
        }

        public Task SetGpsOverrideAsync(int photoId, GpsCoordinate gps, string username)
        {
            return _repo.SetGpsOverrideAsync(photoId, gps, username);
        }

        public async Task SetCategoryTeaserAsync(short categoryId, int photoId)
        {
            var count = await _repo.SetCategoryTeaserAsync(categoryId, photoId).ConfigureAwait(false);

            if (count != 1)
            {
                throw new ApplicationException("Did not update category teaser!");
            }

            await ClearCacheAsync().ConfigureAwait(false);
        }

        public Task ClearCacheAsync()
        {
            return InternalClearCacheAsync();
        }

        static string GetRoleCacheKeyComponent(string[] roles)
        {
            return string.Join("_", roles);
        }
    }
}
