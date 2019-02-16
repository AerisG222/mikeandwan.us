using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Maw.Domain.Photos
{
    public class PhotoService
        : IPhotoService
    {
        readonly IPhotoRepository _repo;


        public PhotoService(IPhotoRepository photoRepository)
        {
            if(photoRepository == null)
            {
				throw new ArgumentNullException(nameof(photoRepository));
            }

            _repo = photoRepository;
        }


        public Task<Photo> GetRandomPhotoAsync(bool allowPrivate)
        {
            return _repo.GetRandomPhotoAsync(allowPrivate);
        }


        public Task<IEnumerable<Photo>> GetRandomPhotosAsync(byte count, bool allowPrivate)
        {
            return _repo.GetRandomPhotosAsync(count, allowPrivate);
        }


		public Task<IEnumerable<short>> GetYearsAsync()
        {
            return _repo.GetYearsAsync();
        }


        public Task<IEnumerable<Category>> GetAllCategoriesAsync(bool allowPrivate)
        {
            return _repo.GetAllCategoriesAsync(allowPrivate);
        }


		public Task<IEnumerable<Category>> GetCategoriesForYearAsync(short year, bool allowPrivate)
        {
            return _repo.GetCategoriesForYearAsync(year, allowPrivate);
        }


		public Task<short> GetCategoryCountAsync(bool allowPrivate)
        {
            return _repo.GetCategoryCountAsync(allowPrivate);
        }


		public Task<IEnumerable<Category>> GetRecentCategoriesAsync(short sinceId, bool allowPrivate)
        {
            return _repo.GetRecentCategoriesAsync(sinceId, allowPrivate);
        }


		public Task<IEnumerable<Photo>> GetPhotosForCategoryAsync(short categoryId, bool allowPrivate)
        {
            return _repo.GetPhotosForCategoryAsync(categoryId, allowPrivate);
        }


		public Task<IEnumerable<Photo>> GetPhotosByCommentDateAsync(bool newestFirst, bool allowPrivate)
        {
            return _repo.GetPhotosByCommentDateAsync(newestFirst, allowPrivate);
        }


		public Task<IEnumerable<Photo>> GetPhotosByUserCommentDateAsync(string username, bool greatestFirst, bool allowPrivate)
        {
            return _repo.GetPhotosByUserCommentDateAsync(username, greatestFirst, allowPrivate);
        }


		public Task<IEnumerable<Photo>> GetPhotosByCommentCountAsync(bool greatestFirst, bool allowPrivate)
        {
            return _repo.GetPhotosByCommentCountAsync(greatestFirst, allowPrivate);
        }


		public Task<IEnumerable<Photo>> GetPhotosByAverageUserRatingAsync(bool highestFirst, bool allowPrivate)
        {
            return _repo.GetPhotosByAverageUserRatingAsync(highestFirst, allowPrivate);
        }


		public Task<IEnumerable<Photo>> GetPhotosByUserRatingAsync(string username, bool highestFirst, bool allowPrivate)
        {
            return _repo.GetPhotosByUserRatingAsync(username, highestFirst, allowPrivate);
        }


		public Task<Category> GetCategoryAsync(short categoryId, bool allowPrivate)
        {
            return _repo.GetCategoryAsync(categoryId, allowPrivate);
        }


		public Task<Photo> GetPhotoAsync(int photoId, bool allowPrivate)
        {
            return _repo.GetPhotoAsync(photoId, allowPrivate);
        }


		public Task<Detail> GetDetailForPhotoAsync(int photoId, bool allowPrivate)
        {
            return _repo.GetDetailForPhotoAsync(photoId, allowPrivate);
        }


		public Task<IEnumerable<Comment>> GetCommentsForPhotoAsync(int photoId)
        {
            return _repo.GetCommentsForPhotoAsync(photoId);
        }


		public Task<Rating> GetRatingsAsync(int photoId, string username)
        {
            return _repo.GetRatingsAsync(photoId, username);
        }


		public Task<int> InsertPhotoCommentAsync(int photoId, string username, string comment)
        {
            return _repo.InsertPhotoCommentAsync(photoId, username, comment);
        }


		public Task<float?> SavePhotoRatingAsync(int photoId, string username, byte rating)
        {
            return _repo.SavePhotoRatingAsync(photoId, username, rating);
        }


		public Task<float?> RemovePhotoRatingAsync(int photoId, string username)
        {
            return _repo.RemovePhotoRatingAsync(photoId, username);
        }
    }
}

