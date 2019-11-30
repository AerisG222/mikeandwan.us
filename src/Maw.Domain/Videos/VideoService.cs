using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;


namespace Maw.Domain.Videos
{
    public class VideoService
        : BaseService, IVideoService
    {
        readonly IVideoRepository _repo;


		public VideoService(IVideoRepository videoRepository,
                            ILogger<VideoService> log,
                            IDistributedCache cache)
            : base("videos", log, cache)
        {
			_repo = videoRepository ?? throw new ArgumentNullException(nameof(videoRepository));
        }


        public Task<IEnumerable<short>> GetYearsAsync(bool allowPrivate)
        {
            var key = $"{nameof(GetYearsAsync)}_{allowPrivate}";

            return GetCachedValueAsync(key, () => _repo.GetYearsAsync(allowPrivate));
        }


        public Task<IEnumerable<Category>> GetAllCategoriesAsync(bool allowPrivate)
        {
            var key = $"{nameof(GetAllCategoriesAsync)}_{allowPrivate}";

            return GetCachedValueAsync(key, () => _repo.GetAllCategoriesAsync(allowPrivate));
        }


        public Task<IEnumerable<Category>> GetCategoriesAsync(short year, bool allowPrivate)
        {
            var key = $"{nameof(GetCategoriesAsync)}_{year}_{allowPrivate}";

            return GetCachedValueAsync(key, () => _repo.GetCategoriesAsync(year, allowPrivate));
        }


        public Task<IEnumerable<Video>> GetVideosInCategoryAsync(short categoryId, bool allowPrivate)
        {
            var key = $"{nameof(GetVideosInCategoryAsync)}_{categoryId}_{allowPrivate}";

            return GetCachedValueAsync(key, () => _repo.GetVideosInCategoryAsync(categoryId, allowPrivate), TimeSpan.FromHours(2));
        }


        public Task<Video> GetVideoAsync(short id, bool allowPrivate)
        {
            var key = $"{nameof(GetVideoAsync)}_{id}_{allowPrivate}";

            return GetCachedValueAsync(key, () => _repo.GetVideoAsync(id, allowPrivate), TimeSpan.FromHours(2));
        }


        public Task<Category> GetCategoryAsync(short categoryId, bool allowPrivate)
        {
            var key = $"{nameof(GetCategoryAsync)}_{categoryId}_{allowPrivate}";

            return GetCachedValueAsync(key, () => _repo.GetCategoryAsync(categoryId, allowPrivate), TimeSpan.FromHours(2));
        }


        public Task<IEnumerable<Comment>> GetCommentsAsync(short videoId)
        {
            return _repo.GetCommentsAsync(videoId);
        }


		public Task<Rating> GetRatingsAsync(short videoId, string username)
        {
            return _repo.GetRatingsAsync(videoId, username);
        }


		public Task<int> InsertCommentAsync(short videoId, string username, string comment)
        {
            return _repo.InsertCommentAsync(videoId, username, comment);
        }


		public Task<float?> SaveRatingAsync(short videoId, string username, short rating)
        {
            return _repo.SaveRatingAsync(videoId, username, rating);
        }


		public Task<float?> RemoveRatingAsync(short videoId, string username)
        {
            return _repo.RemoveRatingAsync(videoId, username);
        }
    }
}

