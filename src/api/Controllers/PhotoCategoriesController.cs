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
        public async Task<ActionResult<PhotoCategory>> GetById(int id)
        {
            short catId = 0;

            try
            {
                catId = Convert.ToInt16(id);
            }
            catch(Exception)
            {
                return NotFound();
            }

            var category = await _svc.GetCategoryAsync(catId, Role.IsAdmin(User));

            if(category == null) {
                return NotFound();
            }

            return BuildPhotoCategory(category);
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
                Self = $"https://something/photo-categories/{c.Id}"
            };
        }
    }
}
