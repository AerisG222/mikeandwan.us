using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Maw.Domain.Photos;
using Maw.Security;
using MawApi.Models;
using MawApi.Models.Photos;
using MawApi.Services.Photos;
using MawApi.ViewModels;
using MawApi.ViewModels.Photos;
using MawApi.ViewModels.LegacyPhotos;


namespace MawApi.Controllers
{
    [ApiController]
    [Authorize]
    [Authorize(Policy.ViewPhotos)]
    [Route("photos")]
    public class PhotosController
        : ControllerBase
    {
        readonly IPhotoService _svc;
        readonly PhotoAdapter _photoAdapter;
        readonly LegacyPhotoAdapter _legacyPhotoAdapter;
        LegacyPhotoCategoryAdapter _legacyPhotoCategoryAdapter;


        public PhotosController(
            IPhotoService photoService,
            PhotoAdapter photoAdapter,
            LegacyPhotoAdapter legacyPhotoAdapter,
            LegacyPhotoCategoryAdapter legacyPhotoCategoryAdapter)
        {
            _svc = photoService ?? throw new ArgumentNullException(nameof(photoService));
            _photoAdapter = photoAdapter ?? throw new ArgumentNullException(nameof(photoAdapter));
            _legacyPhotoAdapter = legacyPhotoAdapter ?? throw new ArgumentNullException(nameof(legacyPhotoAdapter));
            _legacyPhotoCategoryAdapter = legacyPhotoCategoryAdapter ?? throw new ArgumentNullException(nameof(legacyPhotoCategoryAdapter));
        }


        [HttpGet("random")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<PhotoViewModel>> GetRandomPhotoAsync()
        {
            var photo = await _svc.GetRandomPhotoAsync(Role.IsAdmin(User));

            return _photoAdapter.Adapt(photo);
        }


        [HttpGet("random/{count}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<ApiCollection<PhotoViewModel>>> GetRandomPhotosAsync(byte count)
        {
            if(count > 50) {
                return BadRequest();
            }

            var photos = await _svc.GetRandomPhotosAsync(count, Role.IsAdmin(User));

            return new ApiCollection<PhotoViewModel>(_photoAdapter.Adapt(photos).ToList());
        }


        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<MawApi.ViewModels.Photos.PhotoViewModel>> GetByIdAsync(int id)
        {
            var photo = await _svc.GetPhotoAsync(id, Role.IsAdmin(User));

            if(photo == null)
            {
                return NotFound();
            }

            return _photoAdapter.Adapt(photo);
        }


        [HttpGet("{id}/comments")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public Task<ApiCollection<Comment>> GetCommentsAsync(int id)
        {
            return InternalGetCommentsAsync(id);
        }


        [HttpPost("{id}/comments")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<ApiCollection<Comment>> AddCommentAsync(int id, CommentViewModel model)
        {
            // TODO: handle invalid photo id?
            // TODO: remove photoId from commentViewModel?
            await _svc.InsertPhotoCommentAsync(id, User.Identity.Name, model.Comment);

            return await InternalGetCommentsAsync(id);
        }


        [HttpGet("{id}/exif")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Detail>> GetExifAsync(int id)
        {
            var data = await _svc.GetDetailForPhotoAsync(id, Role.IsAdmin(User));

            if(data == null)
            {
                return NotFound();
            }

            return data;
        }


        [HttpGet("{id}/rating")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Rating>> GetRatingAsync(int id)
        {
            var rating = await InternalGetRatingAsync(id);

            if(rating == null)
            {
                return NotFound();
            }

            return rating;
        }


        [HttpPatch("{id}/rating")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Rating>> RatePhotoAsync(int id, UserPhotoRating userRating)
        {
            // TODO: handle invalid photo id?
            // TODO: remove photoId from userPhotoRating?
            if(userRating.Rating < 1)
            {
                await _svc.RemovePhotoRatingAsync(id, User.Identity.Name);
            }
            else if(userRating.Rating <= 5)
            {
                await _svc.SavePhotoRatingAsync(id, User.Identity.Name, userRating.Rating);
            }
            else
            {
                return BadRequest();
            }

            var rating = await InternalGetRatingAsync(id);

            if(rating == null)
            {
                return NotFound();
            }

            return rating;
        }


        async Task<ApiCollection<Comment>> InternalGetCommentsAsync(int id)
        {
            var comments = await _svc.GetCommentsForPhotoAsync(id);

            return new ApiCollection<Comment>(comments.ToList());
        }


        Task<Rating> InternalGetRatingAsync(int id)
        {
            return _svc.GetRatingsAsync(id, User.Identity.Name);
        }


        // *******************************************
        // LEGACY APIS
        // *******************************************
        [HttpGet("getPhotoYears")]
        public async Task<IEnumerable<short>> GetPhotoYears()
        {
            return await _svc.GetYearsAsync();
        }


        [HttpGet("getCategory/{categoryId:int}")]
        public async Task<ActionResult<MawApi.ViewModels.LegacyPhotos.Category>> GetCategory(short categoryId)
        {
            var cat = await _svc.GetCategoryAsync(categoryId, Role.IsAdmin(User));

            if(cat == null)
            {
                return NotFound();
            }

            return _legacyPhotoCategoryAdapter.Adapt(cat);
        }


        [HttpGet("getRandomPhoto")]
        public async Task<PhotoAndCategory> GetRandomPhoto()
        {
            var categories = await _svc.GetAllCategoriesAsync(Role.IsAdmin(User));
            var photo = await _svc.GetRandomPhotoAsync(Role.IsAdmin(User));

            return AssembleLegacyPhotoAndCategory(categories, photo);
        }


        [HttpGet("getRecentCategories/{sinceId:int}")]
        public async Task<IEnumerable<MawApi.ViewModels.LegacyPhotos.Category>> GetRecentCategories(short sinceId)
        {
            var results = await _svc.GetRecentCategoriesAsync(sinceId, Role.IsAdmin(User));

            return _legacyPhotoCategoryAdapter.Adapt(results);
        }


        [HttpGet("getCategoryCount")]
        public async Task<int> GetCategoryCount()
        {
            return await _svc.GetCategoryCountAsync(Role.IsAdmin(User));
        }


        [HttpGet("getCategoriesForYear/{year:int}")]
        public async Task<ActionResult<MawApi.ViewModels.LegacyPhotos.Category[]>> GetCategoriesForYear(short year)
        {
            var cats = await _svc.GetCategoriesForYearAsync(year, Role.IsAdmin(User));

            if(cats == null || cats.Count() == 0)
            {
                return NotFound();
            }

            return _legacyPhotoCategoryAdapter.Adapt(cats).ToArray();
        }


        [HttpGet("getPhotosByCategory/{categoryId:int}")]
        public async Task<ActionResult<MawApi.ViewModels.LegacyPhotos.Photo[]>> GetPhotosByCategory(short categoryId)
        {
            var photos = await _svc.GetPhotosForCategoryAsync(categoryId, Role.IsAdmin(User));

            if(photos == null || photos.Count() == 0)
            {
                return NotFound();
            }

            return _legacyPhotoAdapter.Adapt(photos).ToArray();
        }


        [HttpGet("getPhotosByCommentDate/{newestFirst:alpha}")]
        public async Task<IEnumerable<MawApi.ViewModels.LegacyPhotos.Photo>> GetPhotosByCommentDate(bool newestFirst)
        {
            var results = await _svc.GetPhotosByCommentDateAsync(newestFirst, Role.IsAdmin(User));

            return _legacyPhotoAdapter.Adapt(results);
        }


        [HttpGet("getPhotosByUserCommentDate/{newestFirst:alpha}")]
        public async Task<IEnumerable<MawApi.ViewModels.LegacyPhotos.Photo>> GetPhotosByUserCommentDate(bool newestFirst)
        {
            var results = await _svc.GetPhotosByUserCommentDateAsync(User.Identity.Name, newestFirst, Role.IsAdmin(User));

            return _legacyPhotoAdapter.Adapt(results);
        }


        [HttpGet("getPhotosByCommentCount/{greatestFirst:alpha}")]
        public async Task<IEnumerable<MawApi.ViewModels.LegacyPhotos.Photo>> GetPhotosByCommentCount(bool greatestFirst)
        {
            var results = await _svc.GetPhotosByCommentCountAsync(greatestFirst, Role.IsAdmin(User));

            return _legacyPhotoAdapter.Adapt(results);
        }


        [HttpGet("getPhotosByAverageRating/{highestFirst:alpha}")]
        public async Task<IEnumerable<MawApi.ViewModels.LegacyPhotos.Photo>> GetPhotosByAverageRating(bool highestFirst)
        {
            var results = await _svc.GetPhotosByAverageUserRatingAsync(highestFirst, Role.IsAdmin(User));

            return _legacyPhotoAdapter.Adapt(results);
        }


        [HttpGet("getPhotosByUserRating/{highestFirst:alpha}")]
        public async Task<IEnumerable<MawApi.ViewModels.LegacyPhotos.Photo>> GetPhotosByUserRating(bool highestFirst)
        {
            var results = await _svc.GetPhotosByUserRatingAsync(User.Identity.Name, highestFirst, Role.IsAdmin(User));

            return _legacyPhotoAdapter.Adapt(results);
        }


        [HttpGet("getPhotoExifData/{photoId:int}")]
        public async Task<ActionResult<Detail>> GetPhotoExifData(int photoId)
        {
            var data = await _svc.GetDetailForPhotoAsync(photoId, Role.IsAdmin(User));

            if(data == null)
            {
                return NotFound();
            }

            return data;
        }


        [HttpGet("getCommentsForPhoto/{photoId:int}")]
        public async Task<IEnumerable<Comment>> GetCommentsForPhoto(int photoId)
        {
            return await _svc.GetCommentsForPhotoAsync(photoId);
        }


        [HttpGet("getRatingForPhoto/{id:int}")]
        public async Task<ActionResult<Rating>> GetRatingForPhoto(int id)
        {
            var rating = await _svc.GetRatingsAsync(id, User.Identity.Name);

            if(rating == null)
            {
                return NotFound();
            }

            return rating;
        }


        [HttpPost("ratePhoto")]
        public async Task<float?> RatePhoto([FromBody] [Required] UserPhotoRating userRating)
        {
            if(userRating.Rating < 1)
            {
                return await _svc.RemovePhotoRatingAsync(userRating.PhotoId, User.Identity.Name);
            }
            else if(userRating.Rating <= 5)
            {
                return await _svc.SavePhotoRatingAsync(userRating.PhotoId, User.Identity.Name, userRating.Rating);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(userRating), "rating must be an integer between 1 and 5");
            }
        }


        [HttpPost("addCommentForPhoto")]
        public async Task<bool> AddCommentForPhoto([FromBody] [Required] CommentViewModel comment)
        {
            int result = await _svc.InsertPhotoCommentAsync(comment.PhotoId, User.Identity.Name, comment.Comment);

            return result > 0;
        }


        [HttpGet("getPhotosAndCategoriesByCommentDate/{newestFirst:alpha}")]
        public async Task<IEnumerable<PhotoAndCategory>> GetPhotosAndCategoriesByCommentDate(bool newestFirst)
        {
            var categories = await _svc.GetAllCategoriesAsync(Role.IsAdmin(User));
            var photos = await _svc.GetPhotosByCommentDateAsync(newestFirst, Role.IsAdmin(User));

            return AssembleLegacyPhotoAndCategory(categories, photos);
        }


        [HttpGet("getPhotosAndCategoriesByUserCommentDate/{newestFirst:alpha}")]
        public async Task<IEnumerable<PhotoAndCategory>> GetPhotosAndCategoriesByUserCommentDate(bool newestFirst)
        {
            var categories = await _svc.GetAllCategoriesAsync(Role.IsAdmin(User));
            var photos = await _svc.GetPhotosByUserCommentDateAsync(User.Identity.Name, newestFirst, Role.IsAdmin(User));

            return AssembleLegacyPhotoAndCategory(categories, photos);
        }


        [HttpGet("getPhotosAndCategoriesByCommentCount/{greatestFirst:alpha}")]
        public async Task<IEnumerable<PhotoAndCategory>> GetPhotosAndCategoriesByCommentCount(bool greatestFirst)
        {
            var categories = await _svc.GetAllCategoriesAsync(Role.IsAdmin(User));
            var photos = await _svc.GetPhotosByCommentCountAsync(greatestFirst, Role.IsAdmin(User));

            return AssembleLegacyPhotoAndCategory(categories, photos);
        }


        [HttpGet("getPhotosAndCategoriesByAverageRating/{highestFirst:alpha}")]
        public async Task<IEnumerable<PhotoAndCategory>> GetPhotosAndCategoriesByAverageRating(bool highestFirst)
        {
            var categories = await _svc.GetAllCategoriesAsync(Role.IsAdmin(User));
            var photos = await _svc.GetPhotosByAverageUserRatingAsync(highestFirst, Role.IsAdmin(User));

            return AssembleLegacyPhotoAndCategory(categories, photos);
        }


        [HttpGet("getPhotosAndCategoriesByUserRating/{highestFirst:alpha}")]
        public async Task<IEnumerable<PhotoAndCategory>> GetPhotosAndCategoriesByUserRating(bool highestFirst)
        {
            var categories = await _svc.GetAllCategoriesAsync(Role.IsAdmin(User));
            var photos = await _svc.GetPhotosByUserRatingAsync(User.Identity.Name, highestFirst, Role.IsAdmin(User));

            return AssembleLegacyPhotoAndCategory(categories, photos);
        }


        IEnumerable<PhotoAndCategory> AssembleLegacyPhotoAndCategory(IEnumerable<Maw.Domain.Photos.Category> categories, IEnumerable<Maw.Domain.Photos.Photo> photos)
        {
            return photos.Select(p => AssembleLegacyPhotoAndCategory(categories, p));
        }


        PhotoAndCategory AssembleLegacyPhotoAndCategory(IEnumerable<Maw.Domain.Photos.Category> categories, Maw.Domain.Photos.Photo photo)
        {
            var category = categories.SingleOrDefault(c => c.Id == photo.CategoryId);

            return new PhotoAndCategory {
                Photo = _legacyPhotoAdapter.Adapt(photo),
                Category = _legacyPhotoCategoryAdapter.Adapt(category)
            };
        }
    }
}
