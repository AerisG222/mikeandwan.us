using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Maw.Domain.Photos;
using MawMvcApp.ViewModels;
using MawMvcApp.ViewModels.Photos;
//using MawMvcApp.Filters;

namespace MawMvcApp.Controllers
{
    [Authorize(MawConstants.POLICY_VIEW_PHOTOS)]
    [Route("api/photos")]
    public class PhotoApiController 
        : MawBaseController<PhotoApiController>
    {
        readonly PhotoService _svc;


        bool IsAdmin
        {
            get
            {
                return User.IsInRole(MawConstants.ROLE_ADMIN);
            }
        }


        public PhotoApiController(IAuthorizationService authorizationService, 
                                  ILogger<PhotoApiController> log, 
                                  IPhotoRepository photoRepository)
            : base(authorizationService, log)
        {
            if(photoRepository == null)
            {
                throw new ArgumentNullException(nameof(photoRepository));
            }

            _svc = new PhotoService(photoRepository);
        }


        [HttpGet("getPhotoYears")]
        public async Task<IEnumerable<short>> GetPhotoYears()
        {
            return await _svc.GetYearsAsync();
        }
            
            
        [HttpGet("getCategory/{categoryId:int}")]
        public async Task<Category> GetCategory(short categoryId)
        {
            return await _svc.GetCategoryAsync(categoryId, IsAdmin);
        }


        [HttpGet("getRandomPhoto")]
        public async Task<PhotoAndCategory> GetRandomPhoto()
        {
            return await _svc.GetRandomPhotoAsync(IsAdmin);
        }


        [HttpGet("getRecentCategories/{sinceId:int}")]
        public async Task<IEnumerable<Category>> GetRecentCategories(short sinceId)
        {
            return await _svc.GetRecentCategoriesAsync(sinceId, IsAdmin);
        }


        [HttpGet("getCategoryCount")]
        public async Task<int> GetCategoryCount()
        {
            return await _svc.GetCategoryCountAsync(IsAdmin);
        }


        [HttpGet("getCategoriesForYear/{year:int}")]
        public async Task<IEnumerable<Category>> GetCategoriesForYear(short year)
        {
            return await _svc.GetCategoriesForYearAsync(year, IsAdmin);
        }
        

        [HttpGet("getPhotosByCategory/{categoryId:int}")]
        public async Task<IEnumerable<Photo>> GetPhotosByCategory(short categoryId)
        {
            return await _svc.GetPhotosForCategoryAsync(categoryId, IsAdmin);
        }


        [HttpGet("getPhotosByCommentDate/{newestFirst:alpha}")]
        public async Task<IEnumerable<Photo>> GetPhotosByCommentDate(bool newestFirst)
        {
            return await _svc.GetPhotosByCommentDateAsync(newestFirst, IsAdmin);
        }
        
        
        [HttpGet("getPhotosByUserCommentDate/{newestFirst:alpha}")]
        public async Task<IEnumerable<Photo>> GetPhotosByUserCommentDate(bool newestFirst)
        {
            return await _svc.GetPhotosByUserCommentDateAsync(User.Identity.Name, newestFirst, IsAdmin);
        }
        
        
        [HttpGet("getPhotosByCommentCount/{greatestFirst:alpha}")]
        public async Task<IEnumerable<Photo>> GetPhotosByCommentCount(bool greatestFirst)
        {
            return await _svc.GetPhotosByCommentCountAsync(greatestFirst, IsAdmin);
        }
        
        
        [HttpGet("getPhotosByAverageRating/{highestFirst:alpha}")]
        public async Task<IEnumerable<Photo>> GetPhotosByAverageRating(bool highestFirst)
        {
            return await _svc.GetPhotosByAverageUserRatingAsync(highestFirst, IsAdmin);
        }
        
        
        [HttpGet("getPhotosByUserRating/{highestFirst:alpha}")]
        public async Task<IEnumerable<Photo>> GetPhotosByUserRating(bool highestFirst)
        {
            return await _svc.GetPhotosByUserRatingAsync(User.Identity.Name, highestFirst, IsAdmin);
        }


        [HttpGet("getPhotoExifData/{photoId:int}")]
        public async Task<Detail> GetPhotoExifData(int photoId)
        {
            return await _svc.GetDetailForPhotoAsync(photoId, IsAdmin);
        }


        [HttpGet("getCommentsForPhoto/{photoId:int}")]
        public async Task<IEnumerable<Comment>> GetCommentsForPhoto(int photoId)
        {
            return await _svc.GetCommentsForPhotoAsync(photoId);
        }


        [HttpGet("getRatingForPhoto/{id:int}")]
        public async Task<Rating> GetRatingForPhoto(int id)
        {
            return await _svc.GetRatingsAsync(id, User.Identity.Name);
        }


        [HttpPost("ratePhoto")]
        //[TypeFilter(typeof(ApiAntiforgeryValidationActionFilter))]
        public async Task<float?> RatePhoto([FromBody]UserPhotoRating userRating)
        {
            if(userRating.Rating < 1)
            {
                return await _svc.RemovePhotoRatingAsync(userRating.PhotoId, User.Identity.Name);
            }

            if(userRating.Rating <= 5)
            {
                return await _svc.SavePhotoRatingAsync(userRating.PhotoId, User.Identity.Name, userRating.Rating);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(userRating), "rating must be an integer between 1 and 5");
            }			
        }
        
        
        [HttpPost("addCommentForPhoto")]
        //[TypeFilter(typeof(ApiAntiforgeryValidationActionFilter))]
        public async Task<bool> AddCommentForPhoto([FromBody]CommentViewModel comment)
        {
            int result = await _svc.InsertPhotoCommentAsync(comment.PhotoId, User.Identity.Name, comment.Comment);
            
            return result > 0;
        }
        
        
        [HttpGet("getPhotosAndCategoriesByCommentDate/{newestFirst:alpha}")]
        public async Task<IEnumerable<PhotoAndCategory>> GetPhotosAndCategoriesByCommentDate(bool newestFirst)
        {
            return await _svc.GetPhotosAndCategoriesByCommentDateAsync(newestFirst, IsAdmin);
        }
        
        
        [HttpGet("getPhotosAndCategoriesByUserCommentDate/{newestFirst:alpha}")]
        public async Task<IEnumerable<PhotoAndCategory>> GetPhotosAndCategoriesByUserCommentDate(bool newestFirst)
        {
            return await _svc.GetPhotosAndCategoriesByUserCommentDateAsync(User.Identity.Name, newestFirst, IsAdmin);
        }
        
        
        [HttpGet("getPhotosAndCategoriesByCommentCount/{greatestFirst:alpha}")]
        public async Task<IEnumerable<PhotoAndCategory>> GetPhotosAndCategoriesByCommentCount(bool greatestFirst)
        {
            return await _svc.GetPhotosAndCategoriesByCommentCountAsync(greatestFirst, IsAdmin);
        }
        
        
        [HttpGet("getPhotosAndCategoriesByAverageRating/{highestFirst:alpha}")]
        public async Task<IEnumerable<PhotoAndCategory>> GetPhotosAndCategoriesByAverageRating(bool highestFirst)
        {
            return await _svc.GetPhotosAndCategoriesByAverageUserRatingAsync(highestFirst, IsAdmin);
        }
        
        
        [HttpGet("getPhotosAndCategoriesByUserRating/{highestFirst:alpha}")]
        public async Task<IEnumerable<PhotoAndCategory>> GetPhotosAndCategoriesByUserRating(bool highestFirst)
        {
            return await _svc.GetPhotosAndCategoriesByUserRatingAsync(User.Identity.Name, highestFirst, IsAdmin);
        }
    }
}
