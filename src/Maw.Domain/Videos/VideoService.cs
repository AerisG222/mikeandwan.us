using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Maw.Domain.Videos
{
    public class VideoService
        : IVideoService
    {
        readonly IVideoRepository _repo;


		public VideoService(IVideoRepository videoRepository)
        {
			if(videoRepository == null)
            {
				throw new ArgumentNullException(nameof(videoRepository));
            }

			_repo = videoRepository;
        }


        public Task<IEnumerable<short>> GetYearsAsync(bool allowPrivate)
        {
            return _repo.GetYearsAsync(allowPrivate);
        }


        public Task<IEnumerable<Category>> GetAllCategoriesAsync(bool allowPrivate)
        {
            return _repo.GetAllCategoriesAsync(allowPrivate);
        }


        public Task<IEnumerable<Category>> GetCategoriesAsync(short year, bool allowPrivate)
        {
            return _repo.GetCategoriesAsync(year, allowPrivate);
        }


        public Task<IEnumerable<Video>> GetVideosInCategoryAsync(short categoryId, bool allowPrivate)
        {
            return _repo.GetVideosInCategoryAsync(categoryId, allowPrivate);
        }


        public Task<Video> GetVideoAsync(short id, bool allowPrivate)
        {
            return _repo.GetVideoAsync(id, allowPrivate);
        }


        public Task<Category> GetCategoryAsync(short categoryId, bool allowPrivate)
        {
            return _repo.GetCategoryAsync(categoryId, allowPrivate);
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

