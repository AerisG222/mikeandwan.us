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


        public Task<PhotoAndCategory> GetRandomPhotoAsync(bool allowPrivate)
        {
            return _repo.GetRandomPhotoAsync(allowPrivate);
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


		public async Task<IEnumerable<Photo>> GetPhotosByCommentDateAsync(bool newestFirst, bool allowPrivate)
        {
            var list = await _repo.GetPhotosAndCategoriesByCommentDateAsync(newestFirst, allowPrivate).ConfigureAwait(false);

            return list.Select(x => x.Photo);
        }


		public async Task<IEnumerable<Photo>> GetPhotosByUserCommentDateAsync(string username, bool greatestFirst, bool allowPrivate)
        {
            var list = await _repo.GetPhotosAndCategoriesByUserCommentDateAsync(username, greatestFirst, allowPrivate).ConfigureAwait(false);

            return list.Select(x => x.Photo);
        }


		public async Task<IEnumerable<Photo>> GetPhotosByCommentCountAsync(bool greatestFirst, bool allowPrivate)
        {
            var list = await _repo.GetPhotosAndCategoriesByCommentCountAsync(greatestFirst, allowPrivate).ConfigureAwait(false);

            return list.Select(x => x.Photo);
        }


		public async Task<IEnumerable<Photo>> GetPhotosByAverageUserRatingAsync(bool highestFirst, bool allowPrivate)
        {
            var list = await _repo.GetPhotosAndCategoriesByAverageUserRatingAsync(highestFirst, allowPrivate).ConfigureAwait(false);

            return list.Select(x => x.Photo);
        }


		public async Task<IEnumerable<Photo>> GetPhotosByUserRatingAsync(string username, bool highestFirst, bool allowPrivate)
        {
            var list = await _repo.GetPhotosAndCategoriesByUserRatingAsync(username, highestFirst, allowPrivate).ConfigureAwait(false);

            return list.Select(x => x.Photo);
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

        public Task<IEnumerable<PhotoAndCategory>> GetPhotosAndCategoriesByCommentDateAsync(bool newestFirst, bool allowPrivate)
        {
            return _repo.GetPhotosAndCategoriesByCommentDateAsync(newestFirst, allowPrivate);
        }


		public Task<IEnumerable<PhotoAndCategory>> GetPhotosAndCategoriesByUserCommentDateAsync(string username, bool greatestFirst, bool allowPrivate)
        {
            return _repo.GetPhotosAndCategoriesByUserCommentDateAsync(username, greatestFirst, allowPrivate);
        }


		public Task<IEnumerable<PhotoAndCategory>> GetPhotosAndCategoriesByCommentCountAsync(bool greatestFirst, bool allowPrivate)
        {
            return _repo.GetPhotosAndCategoriesByCommentCountAsync(greatestFirst, allowPrivate);
        }


		public Task<IEnumerable<PhotoAndCategory>> GetPhotosAndCategoriesByAverageUserRatingAsync(bool highestFirst, bool allowPrivate)
        {
            return _repo.GetPhotosAndCategoriesByAverageUserRatingAsync(highestFirst, allowPrivate);
        }


		public Task<IEnumerable<PhotoAndCategory>> GetPhotosAndCategoriesByUserRatingAsync(string username, bool highestFirst, bool allowPrivate)
        {
            return _repo.GetPhotosAndCategoriesByUserRatingAsync(username, highestFirst, allowPrivate);
        }
    }
}

