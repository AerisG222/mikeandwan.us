using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Maw.Security;
using Maw.Domain.Photos;
using MawApi.ViewModels;
using MawApi.ViewModels.Photos;
using MawApi.Services.Photos;

namespace MawApi.Controllers;

[ApiController]
[Authorize]
[Authorize(MawPolicy.ViewPhotos)]
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
    public async Task<ActionResult<ApiCollectionResult<PhotoCategoryViewModel>>> GetAll()
    {
        var categories = await _svc.GetAllCategoriesAsync(User.GetAllRoles());
        var result = _categoryAdapter.Adapt(categories);

        return new ApiCollectionResult<PhotoCategoryViewModel>(result.ToList());
    }

    [HttpGet("recent/{sinceId}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<ApiCollectionResult<PhotoCategoryViewModel>>> GetRecent(short sinceId)
    {
        var categories = await _svc.GetRecentCategoriesAsync(sinceId, User.GetAllRoles());
        var result = _categoryAdapter.Adapt(categories);

        return new ApiCollectionResult<PhotoCategoryViewModel>(result.ToList());
    }

    [HttpGet("{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public Task<ActionResult<PhotoCategoryViewModel>> GetById(short id)
    {
        return GetCategoryInternalAsync(id);
    }

    [HttpGet("{id}/photos")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<ApiCollectionResult<MawApi.ViewModels.Photos.PhotoViewModel>>> GetPhotos(short id)
    {
        var photos = await _svc.GetPhotosForCategoryAsync(id, User.GetAllRoles());

        if(photos == null)
        {
            return NotFound();
        }

        var results = _photoAdapter.Adapt(photos);

        return new ApiCollectionResult<PhotoViewModel>(results.ToList());
    }

    [HttpPatch("{id}/teaser")]
    [Authorize(MawPolicy.AdminPhotos)]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<PhotoCategoryViewModel>> SetTeaserAsync(short id, SetTeaserViewModel model)
    {
        if(model == null)
        {
            return BadRequest();
        }

        try
        {
            await _svc.SetCategoryTeaserAsync(id, model.PhotoId);
        }
        catch
        {
            return BadRequest();
        }

        return await GetCategoryInternalAsync(id);
    }

    async Task<ActionResult<PhotoCategoryViewModel>> GetCategoryInternalAsync(short id)
    {
        var category = await _svc.GetCategoryAsync(id, User.GetAllRoles());

        if(category == null)
        {
            return NotFound();
        }

        return _categoryAdapter.Adapt(category);
    }
}
