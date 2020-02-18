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


        public Task<Photo> GetRandomAsync(bool allowPrivate)
        {
            return _repo.GetRandomAsync(allowPrivate);
        }


        public Task<IEnumerable<Photo>> GetRandomAsync(byte count, bool allowPrivate)
        {
            return _repo.GetRandomAsync(count, allowPrivate);
        }


		public Task<IEnumerable<short>> GetYearsAsync()
        {
            return GetCachedValueAsync(nameof(GetYearsAsync), () => _repo.GetYearsAsync());
        }


        public Task<IEnumerable<Category>> GetAllCategoriesAsync(bool allowPrivate)
        {
            var key = $"{nameof(GetAllCategoriesAsync)}_{allowPrivate}";

            return GetCachedValueAsync(key, () => _repo.GetAllCategoriesAsync(allowPrivate));
        }


		public Task<IEnumerable<Category>> GetCategoriesForYearAsync(short year, bool allowPrivate)
        {
            var key = $"{nameof(GetCategoriesForYearAsync)}_{year}_{allowPrivate}";

            return GetCachedValueAsync(key, () => _repo.GetCategoriesForYearAsync(year, allowPrivate));
        }


		public Task<IEnumerable<Category>> GetRecentCategoriesAsync(short sinceId, bool allowPrivate)
        {
            return _repo.GetRecentCategoriesAsync(sinceId, allowPrivate);
        }


		public Task<IEnumerable<Photo>> GetPhotosForCategoryAsync(short categoryId, bool allowPrivate)
        {
            var key = $"{nameof(GetPhotosForCategoryAsync)}_{categoryId}_{allowPrivate}";

            return GetCachedValueAsync(key, () => _repo.GetPhotosForCategoryAsync(categoryId, allowPrivate), TimeSpan.FromHours(2));
        }


		public Task<Category> GetCategoryAsync(short categoryId, bool allowPrivate)
        {
            var key = $"{nameof(GetCategoryAsync)}_{categoryId}_{allowPrivate}";

            return GetCachedValueAsync(key, () => _repo.GetCategoryAsync(categoryId, allowPrivate), TimeSpan.FromHours(2));
        }


		public Task<Photo> GetPhotoAsync(int photoId, bool allowPrivate)
        {
            var key = $"{nameof(GetPhotoAsync)}_{photoId}_{allowPrivate}";

            return GetCachedValueAsync(key, () => _repo.GetPhotoAsync(photoId, allowPrivate), TimeSpan.FromHours(2));
        }


		public Task<Detail> GetDetailAsync(int photoId, bool allowPrivate)
        {
            var key = $"{nameof(GetDetailAsync)}_{photoId}_{allowPrivate}";

            return GetCachedValueAsync(key, () => _repo.GetDetailAsync(photoId, allowPrivate), TimeSpan.FromMinutes(15));
        }


		public Task<IEnumerable<Comment>> GetCommentsAsync(int photoId)
        {
            return _repo.GetCommentsAsync(photoId);
        }


		public Task<Rating> GetRatingsAsync(int photoId, string username)
        {
            return _repo.GetRatingsAsync(photoId, username);
        }


        public Task<GpsDetail> GetGpsDetailAsync(int photoId)
        {
            return _repo.GetGpsDetailAsync(photoId);
        }


		public Task<int> InsertCommentAsync(int photoId, string username, string comment)
        {
            return _repo.InsertCommentAsync(photoId, username, comment);
        }


		public Task<float?> SaveRatingAsync(int photoId, string username, short rating)
        {
            return _repo.SaveRatingAsync(photoId, username, rating);
        }


		public Task<float?> RemoveRatingAsync(int photoId, string username)
        {
            return _repo.RemoveRatingAsync(photoId, username);
        }


        public Task SetGpsOverrideAsync(int photoId, GpsCoordinate gps, string username)
        {
            return _repo.SetGpsOverrideAsync(photoId, gps, username);
        }


        public Task ClearCacheAsync()
        {
            return InternalClearCacheAsync();
        }
    }
}
