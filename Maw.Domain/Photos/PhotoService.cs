using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Maw.Domain.Photos
{
    public class PhotoService
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


        public Task<PhotoAndCategory> GetRandomPhotoAsync(bool allowPrivate)
        {
            return _repo.GetRandomPhotoAsync(allowPrivate);
        }


		public Task<List<short>> GetYearsAsync()
        {
            return _repo.GetYearsAsync();
        }


		public Task<List<Category>> GetCategoriesForYearAsync(short year, bool allowPrivate)
        {
            return _repo.GetCategoriesForYearAsync(year, allowPrivate);
        }


		public Task<short> GetCategoryCountAsync(bool allowPrivate)
        {
            return _repo.GetCategoryCountAsync(allowPrivate);
        }


		public Task<List<Category>> GetRecentCategoriesAsync(short sinceId, bool allowPrivate)
        {
            return _repo.GetRecentCategoriesAsync(sinceId, allowPrivate);
        }


		public Task<List<Photo>> GetPhotosForCategoryAsync(short categoryId, bool allowPrivate)
        {
            return _repo.GetPhotosForCategoryAsync(categoryId, allowPrivate);
        }


		public Task<List<Photo>> GetPhotosByCommentDateAsync(bool newestFirst, bool allowPrivate)
        {
            return _repo.GetPhotosByCommentDateAsync(newestFirst, allowPrivate);
        }


		public Task<List<Photo>> GetPhotosByUserCommentDateAsync(string username, bool greatestFirst, bool allowPrivate)
        {
            return _repo.GetPhotosByUserCommentDateAsync(username, greatestFirst, allowPrivate);
        }


		public Task<List<Photo>> GetPhotosByCommentCountAsync(bool greatestFirst, bool allowPrivate)
        {
            return _repo.GetPhotosByCommentCountAsync(greatestFirst, allowPrivate);
        }


		public Task<List<Photo>> GetPhotosByAverageUserRatingAsync(bool highestFirst, bool allowPrivate)
        {
            return _repo.GetPhotosByAverageUserRatingAsync(highestFirst, allowPrivate);
        }


		public Task<List<Photo>> GetPhotosByUserRatingAsync(string username, bool highestFirst, bool allowPrivate)
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


		public Task<List<GpsData>> GetGpsDataForCategoryAsync(short categoryId, bool allowPrivate)
        {
            return _repo.GetGpsDataForCategoryAsync(categoryId, allowPrivate);
        }


		public Task<List<Comment>> GetCommentsForPhotoAsync(int photoId)
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
        
        public Task<List<PhotoAndCategory>> GetPhotosAndCategoriesByCommentDateAsync(bool newestFirst, bool allowPrivate)
        {
            return _repo.GetPhotosAndCategoriesByCommentDateAsync(newestFirst, allowPrivate);
        }


		public Task<List<PhotoAndCategory>> GetPhotosAndCategoriesByUserCommentDateAsync(string username, bool greatestFirst, bool allowPrivate)
        {
            return _repo.GetPhotosAndCategoriesByUserCommentDateAsync(username, greatestFirst, allowPrivate);
        }


		public Task<List<PhotoAndCategory>> GetPhotosAndCategoriesByCommentCountAsync(bool greatestFirst, bool allowPrivate)
        {
            return _repo.GetPhotosAndCategoriesByCommentCountAsync(greatestFirst, allowPrivate);
        }


		public Task<List<PhotoAndCategory>> GetPhotosAndCategoriesByAverageUserRatingAsync(bool highestFirst, bool allowPrivate)
        {
            return _repo.GetPhotosAndCategoriesByAverageUserRatingAsync(highestFirst, allowPrivate);
        }


		public Task<List<PhotoAndCategory>> GetPhotosAndCategoriesByUserRatingAsync(string username, bool highestFirst, bool allowPrivate)
        {
            return _repo.GetPhotosAndCategoriesByUserRatingAsync(username, highestFirst, allowPrivate);
        }
    }
}

