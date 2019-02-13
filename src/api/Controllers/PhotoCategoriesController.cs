using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Maw.Security;
using Maw.Domain.Photos;
using api.Models.Photos;


namespace api.Controllers
{
    [ApiController]
    //[Authorize]
    //[Authorize(Policy.ViewPhotos)]
    [Route("photo-categories")]
    public class PhotoCategoriesController
        : ControllerBase
    {
        readonly IPhotoService _svc;


        public PhotoCategoriesController(IPhotoService photoService)
        {
            _svc = photoService ?? throw new ArgumentNullException(nameof(photoService));
        }


        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<List<PhotoCategory>>> Get()
        {
            var categories = await _svc.GetAllCategoriesAsync(Role.IsAdmin(User));
            var result = categories.Select(c => BuildPhotoCategory(c));

            return result.ToList();
        }


        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<PhotoCategory>> GetById(short id)
        {
            var category = await _svc.GetCategoryAsync(id, Role.IsAdmin(User));

            if(category == null)
            {
                return NotFound();
            }

            return BuildPhotoCategory(category);
        }


        [HttpGet("{id}/photos")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<List<api.Models.Photos.Photo>>> GetPhotos(short id)
        {
            var photos = await _svc.GetPhotosForCategoryAsync(id, Role.IsAdmin(User));

            if(photos == null)
            {
                return NotFound();
            }

            var results = photos.Select(p => BuildPhoto(p));

            return results.ToList();
        }


        PhotoCategory BuildPhotoCategory(Category c)
        {
            return new PhotoCategory {
                Id = c.Id,
                Name = c.Name,
                Year = c.Year,
                HasGpsData = c.HasGpsData,
                PhotoCount = c.PhotoCount,
                TeaserPhotoInfo = c.TeaserPhotoInfo,
                Self = GetAbsoluteLink($"photo-categories/{c.Id}")
            };
        }


        api.Models.Photos.Photo BuildPhoto(Maw.Domain.Photos.Photo p)
        {
            return new api.Models.Photos.Photo {
                Id = p.Id,
                CategoryId = p.CategoryId,
                Latitude = p.Latitude,
                Longitude = p.Longitude,
                XsInfo = p.XsInfo,
                SmInfo = p.SmInfo,
                MdInfo = p.MdInfo,
                LgInfo = p.LgInfo,
                PrtInfo = p.PrtInfo,
                Self = GetPhotoLink(p.Id),
                CategoryLink = GetPhotoCategoryLink(p.CategoryId)
            };
        }


        string GetPhotoCategoryLink(short categoryId)
        {
            return GetAbsoluteLink($"photo-categories/{categoryId}");
        }


        string GetPhotoLink(int photoId)
        {
            return GetAbsoluteLink($"photos/{photoId}");
        }


        string GetAbsoluteLink(string relativePath)
        {
            return $"https://something/{relativePath}";
        }
    }
}
