using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Maw.Security;
using Maw.Domain.Videos;
using MawApi.Services.Videos;
using MawApi.ViewModels;
using MawApi.ViewModels.Videos;

namespace MawApi.Controllers;

[ApiController]
[Authorize]
[Authorize(MawPolicy.ViewPhotos)]
[Route("video-categories")]
public class VideoCategoriesController
    : ControllerBase
{
    readonly IVideoService _svc;
    readonly VideoAdapter _videoAdapter;
    readonly VideoCategoryAdapter _categoryAdapter;

    public VideoCategoriesController(
        IVideoService videoService,
        VideoAdapter videoAdapter,
        VideoCategoryAdapter categoryAdapter)
    {
        _svc = videoService ?? throw new ArgumentNullException(nameof(videoService));
        _videoAdapter = videoAdapter ?? throw new ArgumentNullException(nameof(videoAdapter));
        _categoryAdapter = categoryAdapter ?? throw new ArgumentNullException(nameof(categoryAdapter));
    }

    [HttpGet]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<ApiCollectionResult<VideoCategoryViewModel>>> GetAll()
    {
        var categories = await _svc.GetAllCategoriesAsync(User.GetAllRoles()).ConfigureAwait(false);
        var result = _categoryAdapter.Adapt(categories);

        return new ApiCollectionResult<VideoCategoryViewModel>(result.ToList());
    }

    [HttpGet("{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public Task<ActionResult<VideoCategoryViewModel>> GetById(short id)
    {
        return GetCategoryInternalAsync(id);
    }

    [HttpGet("{id}/videos")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<ApiCollectionResult<MawApi.ViewModels.Videos.VideoViewModel>>> GetVideos(short id)
    {
        var photos = await _svc.GetVideosInCategoryAsync(id, User.GetAllRoles()).ConfigureAwait(false);

        if(photos == null)
        {
            return NotFound();
        }

        var results = _videoAdapter.Adapt(photos);

        return new ApiCollectionResult<VideoViewModel>(results.ToList());
    }

    [HttpPatch("{id}/teaser")]
    [Authorize(MawPolicy.AdminVideos)]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<VideoCategoryViewModel>> SetTeaserAsync(short id, SetTeaserViewModel model)
    {
        if(model == null)
        {
            return BadRequest();
        }

        try
        {
            await _svc.SetCategoryTeaserAsync(id, model.VideoId).ConfigureAwait(false);
        }
        catch
        {
            return BadRequest();
        }

        return await GetCategoryInternalAsync(id).ConfigureAwait(false);
    }

    async Task<ActionResult<VideoCategoryViewModel>> GetCategoryInternalAsync(short id)
    {
        var category = await _svc.GetCategoryAsync(id, User.GetAllRoles()).ConfigureAwait(false);

        if(category == null)
        {
            return NotFound();
        }

        return _categoryAdapter.Adapt(category);
    }
}
