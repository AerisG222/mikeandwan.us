using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Maw.Domain;
using Maw.Domain.Photos;
using Maw.Security;
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
            var photo = await _svc.GetRandomAsync(Role.IsAdmin(User));

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

            var photos = await _svc.GetRandomAsync(count, Role.IsAdmin(User));

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
            await _svc.InsertCommentAsync(id, User.Identity.Name, model.Comment);

            return await InternalGetCommentsAsync(id);
        }


        [HttpGet("{id}/exif")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Detail>> GetExifAsync(int id)
        {
            var data = await _svc.GetDetailAsync(id, Role.IsAdmin(User));

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
        public async Task<ActionResult<Rating>> RatePhotoAsync(int id, UserRating userRating)
        {
            // TODO: handle invalid photo id?
            // TODO: remove photoId from userPhotoRating?
            if(userRating.Rating < 1)
            {
                await _svc.RemoveRatingAsync(id, User.Identity.Name);
            }
            else if(userRating.Rating <= 5)
            {
                await _svc.SaveRatingAsync(id, User.Identity.Name, userRating.Rating);
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
            var comments = await _svc.GetCommentsAsync(id);

            return new ApiCollection<Comment>(comments.ToList());
        }


        Task<Rating> InternalGetRatingAsync(int id)
        {
            return _svc.GetRatingsAsync(id, User.Identity.Name);
        }


        // *******************************************
        // LEGACY APIS
        // *******************************************
        [Obsolete]
        [HttpGet("getPhotoYears")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<IEnumerable<short>> GetPhotoYears()
        {
            return await _svc.GetYearsAsync();
        }


        [Obsolete]
        [HttpGet("getCategory/{categoryId:int}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<MawApi.ViewModels.LegacyPhotos.Category>> GetCategory(short categoryId)
        {
            var cat = await _svc.GetCategoryAsync(categoryId, Role.IsAdmin(User));

            if(cat == null)
            {
                return NotFound();
            }

            return _legacyPhotoCategoryAdapter.Adapt(cat);
        }


        [Obsolete]
        [HttpGet("getRandomPhoto")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<PhotoAndCategory> GetRandomPhoto()
        {
            var categories = await _svc.GetAllCategoriesAsync(Role.IsAdmin(User));
            var photo = await _svc.GetRandomAsync(Role.IsAdmin(User));

            return AssembleLegacyPhotoAndCategory(categories, photo);
        }


        [Obsolete]
        [HttpGet("getRecentCategories/{sinceId:int}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<IEnumerable<MawApi.ViewModels.LegacyPhotos.Category>> GetRecentCategories(short sinceId)
        {
            var results = await _svc.GetRecentCategoriesAsync(sinceId, Role.IsAdmin(User));

            return _legacyPhotoCategoryAdapter.Adapt(results);
        }


        [Obsolete]
        [HttpGet("getCategoryCount")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<int> GetCategoryCount()
        {
            return await _svc.GetCategoryCountAsync(Role.IsAdmin(User));
        }


        [Obsolete]
        [HttpGet("getCategoriesForYear/{year:int}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<MawApi.ViewModels.LegacyPhotos.Category[]>> GetCategoriesForYear(short year)
        {
            var cats = await _svc.GetCategoriesForYearAsync(year, Role.IsAdmin(User));

            if(cats == null || cats.Count() == 0)
            {
                return NotFound();
            }

            return _legacyPhotoCategoryAdapter.Adapt(cats).ToArray();
        }


        [Obsolete]
        [HttpGet("getPhotosByCategory/{categoryId:int}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<MawApi.ViewModels.LegacyPhotos.Photo[]>> GetPhotosByCategory(short categoryId)
        {
            var photos = await _svc.GetPhotosForCategoryAsync(categoryId, Role.IsAdmin(User));

            if(photos == null || photos.Count() == 0)
            {
                return NotFound();
            }

            return _legacyPhotoAdapter.Adapt(photos).ToArray();
        }


        [Obsolete]
        [HttpGet("getPhotosByCommentDate/{newestFirst:alpha}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<IEnumerable<MawApi.ViewModels.LegacyPhotos.Photo>> GetPhotosByCommentDate(bool newestFirst)
        {
            var results = await _svc.GetPhotosByCommentDateAsync(newestFirst, Role.IsAdmin(User));

            return _legacyPhotoAdapter.Adapt(results);
        }


        [Obsolete]
        [HttpGet("getPhotosByUserCommentDate/{newestFirst:alpha}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<IEnumerable<MawApi.ViewModels.LegacyPhotos.Photo>> GetPhotosByUserCommentDate(bool newestFirst)
        {
            var results = await _svc.GetPhotosByUserCommentDateAsync(User.Identity.Name, newestFirst, Role.IsAdmin(User));

            return _legacyPhotoAdapter.Adapt(results);
        }


        [Obsolete]
        [HttpGet("getPhotosByCommentCount/{greatestFirst:alpha}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<IEnumerable<MawApi.ViewModels.LegacyPhotos.Photo>> GetPhotosByCommentCount(bool greatestFirst)
        {
            var results = await _svc.GetPhotosByCommentCountAsync(greatestFirst, Role.IsAdmin(User));

            return _legacyPhotoAdapter.Adapt(results);
        }


        [Obsolete]
        [HttpGet("getPhotosByAverageRating/{highestFirst:alpha}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<IEnumerable<MawApi.ViewModels.LegacyPhotos.Photo>> GetPhotosByAverageRating(bool highestFirst)
        {
            var results = await _svc.GetPhotosByAverageUserRatingAsync(highestFirst, Role.IsAdmin(User));

            return _legacyPhotoAdapter.Adapt(results);
        }


        [Obsolete]
        [HttpGet("getPhotosByUserRating/{highestFirst:alpha}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<IEnumerable<MawApi.ViewModels.LegacyPhotos.Photo>> GetPhotosByUserRating(bool highestFirst)
        {
            var results = await _svc.GetPhotosByUserRatingAsync(User.Identity.Name, highestFirst, Role.IsAdmin(User));

            return _legacyPhotoAdapter.Adapt(results);
        }


        [Obsolete]
        [HttpGet("getPhotoExifData/{photoId:int}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Detail>> GetPhotoExifData(int photoId)
        {
            var data = await _svc.GetDetailAsync(photoId, Role.IsAdmin(User));

            if(data == null)
            {
                return NotFound();
            }

            return data;
        }


        [Obsolete]
        [HttpGet("getCommentsForPhoto/{photoId:int}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IEnumerable<Comment>> GetCommentsForPhoto(int photoId)
        {
            return await _svc.GetCommentsAsync(photoId);
        }


        [Obsolete]
        [HttpGet("getRatingForPhoto/{id:int}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Rating>> GetRatingForPhoto(int id)
        {
            var rating = await _svc.GetRatingsAsync(id, User.Identity.Name);

            if(rating == null)
            {
                return NotFound();
            }

            return rating;
        }


        [Obsolete]
        [HttpPost("ratePhoto")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<float?> RatePhoto([FromBody] [Required] UserRating userRating)
        {
            if(userRating.Rating < 1)
            {
                return await _svc.RemoveRatingAsync(userRating.PhotoId, User.Identity.Name);
            }
            else if(userRating.Rating <= 5)
            {
                return await _svc.SaveRatingAsync(userRating.PhotoId, User.Identity.Name, userRating.Rating);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(userRating), "rating must be an integer between 1 and 5");
            }
        }


        [Obsolete]
        [HttpPost("addCommentForPhoto")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<bool> AddCommentForPhoto([FromBody] [Required] CommentViewModel comment)
        {
            int result = await _svc.InsertCommentAsync(comment.PhotoId, User.Identity.Name, comment.Comment);

            return result > 0;
        }


        [Obsolete]
        [HttpGet("getPhotosAndCategoriesByCommentDate/{newestFirst:alpha}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<IEnumerable<PhotoAndCategory>> GetPhotosAndCategoriesByCommentDate(bool newestFirst)
        {
            var categories = await _svc.GetAllCategoriesAsync(Role.IsAdmin(User));
            var photos = await _svc.GetPhotosByCommentDateAsync(newestFirst, Role.IsAdmin(User));

            return AssembleLegacyPhotoAndCategory(categories, photos);
        }


        [Obsolete]
        [HttpGet("getPhotosAndCategoriesByUserCommentDate/{newestFirst:alpha}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<IEnumerable<PhotoAndCategory>> GetPhotosAndCategoriesByUserCommentDate(bool newestFirst)
        {
            var categories = await _svc.GetAllCategoriesAsync(Role.IsAdmin(User));
            var photos = await _svc.GetPhotosByUserCommentDateAsync(User.Identity.Name, newestFirst, Role.IsAdmin(User));

            return AssembleLegacyPhotoAndCategory(categories, photos);
        }


        [Obsolete]
        [HttpGet("getPhotosAndCategoriesByCommentCount/{greatestFirst:alpha}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<IEnumerable<PhotoAndCategory>> GetPhotosAndCategoriesByCommentCount(bool greatestFirst)
        {
            var categories = await _svc.GetAllCategoriesAsync(Role.IsAdmin(User));
            var photos = await _svc.GetPhotosByCommentCountAsync(greatestFirst, Role.IsAdmin(User));

            return AssembleLegacyPhotoAndCategory(categories, photos);
        }


        [Obsolete]
        [HttpGet("getPhotosAndCategoriesByAverageRating/{highestFirst:alpha}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<IEnumerable<PhotoAndCategory>> GetPhotosAndCategoriesByAverageRating(bool highestFirst)
        {
            var categories = await _svc.GetAllCategoriesAsync(Role.IsAdmin(User));
            var photos = await _svc.GetPhotosByAverageUserRatingAsync(highestFirst, Role.IsAdmin(User));

            return AssembleLegacyPhotoAndCategory(categories, photos);
        }


        [Obsolete]
        [HttpGet("getPhotosAndCategoriesByUserRating/{highestFirst:alpha}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
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
