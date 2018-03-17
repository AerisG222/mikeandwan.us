using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Maw.Domain.Photos;
using Maw.Domain.Photos.ThreeD;
using MawApi.ViewModels;
using MawApi.ViewModels.Photos;
using MawApi.ViewModels.Photos.Stats;
using Maw.Security;
using System.Linq;
using System.ComponentModel.DataAnnotations;


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


        public PhotosController(IPhotoService photoService)
        {
            _svc = photoService ?? throw new ArgumentNullException(nameof(photoService));
        }


        [HttpGet("getPhotoYears")]
        public async Task<IEnumerable<short>> GetPhotoYears()
        {
            return await _svc.GetYearsAsync();
        }


        [HttpGet("getCategory/{categoryId:int}")]
        public async Task<ActionResult<Category>> GetCategory(short categoryId)
        {
            var cat = await _svc.GetCategoryAsync(categoryId, Role.IsAdmin(User));

            if(cat == null)
            {
                return NotFound();
            }

            return cat;
        }


        [HttpGet("getRandomPhoto")]
        public async Task<PhotoAndCategory> GetRandomPhoto()
        {
            return await _svc.GetRandomPhotoAsync(Role.IsAdmin(User));
        }


        [HttpGet("getRecentCategories/{sinceId:int}")]
        public async Task<IEnumerable<Category>> GetRecentCategories(short sinceId)
        {
            return await _svc.GetRecentCategoriesAsync(sinceId, Role.IsAdmin(User));
        }


        [HttpGet("getCategoryCount")]
        public async Task<int> GetCategoryCount()
        {
            return await _svc.GetCategoryCountAsync(Role.IsAdmin(User));
        }


        [HttpGet("getCategoriesForYear/{year:int}")]
        public async Task<ActionResult<Category[]>> GetCategoriesForYear(short year)
        {
            var cats = await _svc.GetCategoriesForYearAsync(year, Role.IsAdmin(User));

            if(cats == null || cats.Count() == 0)
            {
                return NotFound();
            }

            return cats.ToArray();
        }


        [HttpGet("getPhotosByCategory/{categoryId:int}")]
        public async Task<ActionResult<Photo[]>> GetPhotosByCategory(short categoryId)
        {
            var photos = await _svc.GetPhotosForCategoryAsync(categoryId, Role.IsAdmin(User));

            if(photos == null || photos.Count() == 0)
            {
                return NotFound();
            }

            return photos.ToArray();
        }


        [HttpGet("getPhotosByCommentDate/{newestFirst:alpha}")]
        public async Task<IEnumerable<Photo>> GetPhotosByCommentDate(bool newestFirst)
        {
            return await _svc.GetPhotosByCommentDateAsync(newestFirst, Role.IsAdmin(User));
        }


        [HttpGet("getPhotosByUserCommentDate/{newestFirst:alpha}")]
        public async Task<IEnumerable<Photo>> GetPhotosByUserCommentDate(bool newestFirst)
        {
            return await _svc.GetPhotosByUserCommentDateAsync(User.Identity.Name, newestFirst, Role.IsAdmin(User));
        }


        [HttpGet("getPhotosByCommentCount/{greatestFirst:alpha}")]
        public async Task<IEnumerable<Photo>> GetPhotosByCommentCount(bool greatestFirst)
        {
            return await _svc.GetPhotosByCommentCountAsync(greatestFirst, Role.IsAdmin(User));
        }


        [HttpGet("getPhotosByAverageRating/{highestFirst:alpha}")]
        public async Task<IEnumerable<Photo>> GetPhotosByAverageRating(bool highestFirst)
        {
            return await _svc.GetPhotosByAverageUserRatingAsync(highestFirst, Role.IsAdmin(User));
        }


        [HttpGet("getPhotosByUserRating/{highestFirst:alpha}")]
        public async Task<IEnumerable<Photo>> GetPhotosByUserRating(bool highestFirst)
        {
            return await _svc.GetPhotosByUserRatingAsync(User.Identity.Name, highestFirst, Role.IsAdmin(User));
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
            return await _svc.GetPhotosAndCategoriesByCommentDateAsync(newestFirst, Role.IsAdmin(User));
        }


        [HttpGet("getPhotosAndCategoriesByUserCommentDate/{newestFirst:alpha}")]
        public async Task<IEnumerable<PhotoAndCategory>> GetPhotosAndCategoriesByUserCommentDate(bool newestFirst)
        {
            return await _svc.GetPhotosAndCategoriesByUserCommentDateAsync(User.Identity.Name, newestFirst, Role.IsAdmin(User));
        }


        [HttpGet("getPhotosAndCategoriesByCommentCount/{greatestFirst:alpha}")]
        public async Task<IEnumerable<PhotoAndCategory>> GetPhotosAndCategoriesByCommentCount(bool greatestFirst)
        {
            return await _svc.GetPhotosAndCategoriesByCommentCountAsync(greatestFirst, Role.IsAdmin(User));
        }


        [HttpGet("getPhotosAndCategoriesByAverageRating/{highestFirst:alpha}")]
        public async Task<IEnumerable<PhotoAndCategory>> GetPhotosAndCategoriesByAverageRating(bool highestFirst)
        {
            return await _svc.GetPhotosAndCategoriesByAverageUserRatingAsync(highestFirst, Role.IsAdmin(User));
        }


        [HttpGet("getPhotosAndCategoriesByUserRating/{highestFirst:alpha}")]
        public async Task<IEnumerable<PhotoAndCategory>> GetPhotosAndCategoriesByUserRating(bool highestFirst)
        {
            return await _svc.GetPhotosAndCategoriesByUserRatingAsync(User.Identity.Name, highestFirst, Role.IsAdmin(User));
        }


        [HttpGet("getStats")]
        public async Task<IEnumerable<YearStats>> GetStats()
        {
            short year = 0;
            YearStats currYearStat = null;
            var yearStats = new List<YearStats>();
            var stats = await _svc.GetStats(Role.IsAdmin(User));

            foreach(var stat in stats)
            {
                if(year != stat.Year)
                {
                    year = stat.Year;

                    currYearStat = new YearStats
                    {
                        Year = year
                    };

                    yearStats.Add(currYearStat);
                }

                currYearStat.CategoryStats.Add(new CategoryStats
                    {
                        Id = stat.CategoryId,
                        Name = stat.CategoryName,
                        PhotoCount = stat.PhotoCount
                    });
            }

            return yearStats;
        }


        [HttpGet("getAllCategories3D")]
        public async Task<IEnumerable<Category3D>> GetAllCategories3D()
        {
            return await _svc.GetAllCategories3D();
        }


        [HttpGet("getPhotos3D/{categoryId:int}")]
        public async Task<ActionResult<Photo3D[]>> GetPhotos3D(int categoryId)
        {
            var photos = await _svc.GetPhotos3D(categoryId);

            if(photos == null)
            {
                return NotFound();
            }

            return photos.ToArray();
        }
    }
}
