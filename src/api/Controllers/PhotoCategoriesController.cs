using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Maw.Security;
using Maw.Domain.Photos;
using MawApi.ViewModels;
using MawApi.ViewModels.Photos;
using MawApi.Services.Photos;


namespace MawApi.Controllers
{
    [ApiController]
    [Authorize]
    [Authorize(Policy.ViewPhotos)]
    [Route("photo-categories")]
    public class PhotoCategoriesController
        : ControllerBase
    {
        readonly IPhotoService _svc;
        readonly PhotoAdapter _photoAdapter;
        readonly PhotoCategoryAdapter _categoryAdapter;


        public PhotoCategoriesController(
            IPhotoService photoService,
            PhotoAdapter photoAdapter,
            PhotoCategoryAdapter categoryAdapter)
        {
            _svc = photoService ?? throw new ArgumentNullException(nameof(photoService));
            _photoAdapter = photoAdapter ?? throw new ArgumentNullException(nameof(photoAdapter));
            _categoryAdapter = categoryAdapter ?? throw new ArgumentNullException(nameof(categoryAdapter));
        }


        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<ApiCollection<PhotoCategoryViewModel>>> GetAll()
        {
            var categories = await _svc.GetAllCategoriesAsync(Role.IsAdmin(User));
            var result = _categoryAdapter.Adapt(categories);

            return new ApiCollection<PhotoCategoryViewModel>(result.ToList());
        }


        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<PhotoCategoryViewModel>> GetById(short id)
        {
            var category = await _svc.GetCategoryAsync(id, Role.IsAdmin(User));

            if(category == null)
            {
                return NotFound();
            }

            return _categoryAdapter.Adapt(category);
        }


        [HttpGet("{id}/photos")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ApiCollection<MawApi.ViewModels.Photos.PhotoViewModel>>> GetPhotos(short id)
        {
            var photos = await _svc.GetPhotosForCategoryAsync(id, Role.IsAdmin(User));

            if(photos == null)
            {
                return NotFound();
            }

            var results = _photoAdapter.Adapt(photos);

            return new ApiCollection<PhotoViewModel>(results.ToList());
        }
    }
}
