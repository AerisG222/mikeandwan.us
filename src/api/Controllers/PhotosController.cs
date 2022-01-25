using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Maw.Domain;
using Maw.Domain.Models;
using Maw.Domain.Models.Photos;
using Maw.Domain.Photos;
using Maw.Security;
using MawApi.Models.Photos;
using MawApi.Services.Photos;
using MawApi.ViewModels;
using MawApi.ViewModels.Photos;

namespace MawApi.Controllers;

[ApiController]
[Authorize]
[Authorize(MawPolicy.ViewPhotos)]
[Route("photos")]
public class PhotosController
    : ControllerBase
{
    readonly IPhotoService _svc;
    readonly PhotoAdapter _photoAdapter;

    public PhotosController(
        IPhotoService photoService,
        PhotoAdapter photoAdapter)
    {
        _svc = photoService ?? throw new ArgumentNullException(nameof(photoService));
        _photoAdapter = photoAdapter ?? throw new ArgumentNullException(nameof(photoAdapter));
    }

    [HttpGet("random")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<PhotoViewModel>> GetRandomPhotoAsync()
    {
        var photo = await _svc.GetRandomAsync(User.GetAllRoles());

        if(photo == null)
        {
            return NotFound();
        }

        return _photoAdapter.Adapt(photo);
    }

    [HttpGet("random/{count}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<ApiCollectionResult<PhotoViewModel>>> GetRandomPhotosAsync(byte count)
    {
        if(count > 50) {
            return BadRequest();
        }

        var photos = await _svc.GetRandomAsync(count, User.GetAllRoles());

        return new ApiCollectionResult<PhotoViewModel>(_photoAdapter.Adapt(photos).ToList());
    }

    [HttpGet("{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<PhotoViewModel>> GetByIdAsync(int id)
    {
        var photo = await _svc.GetPhotoAsync(id, User.GetAllRoles());

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
    public Task<ApiCollectionResult<Comment>> GetCommentsAsync(int id)
    {
        return InternalGetCommentsAsync(id);
    }

    [HttpPost("{id}/comments")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<ApiCollectionResult<Comment>> AddCommentAsync(int id, CommentViewModel model)
    {
        if(model == null)
        {
            throw new ArgumentNullException(nameof(model));
        }

        // TODO: handle invalid photo id?
        // TODO: remove photoId from commentViewModel?
        await _svc.InsertCommentAsync(id, User.GetUsername(), model.Comment, User.GetAllRoles());

        return await InternalGetCommentsAsync(id);
    }

    [HttpGet("{id}/exif")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<Detail>> GetExifAsync(int id)
    {
        var data = await _svc.GetDetailAsync(id, User.GetAllRoles());

        if(data == null)
        {
            return NotFound();
        }

        return data;
    }

    [HttpGet("{id}/gps")]
    [Authorize(MawPolicy.AdminPhotos)]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<GpsDetail>> GetGpsAsync(int id)
    {
        var gps = await _svc.GetGpsDetailAsync(id, User.GetAllRoles());

        if(gps == null)
        {
            return NotFound();
        }

        return gps;
    }

    [HttpPatch("{id}/gps")]
    [Authorize(MawPolicy.AdminPhotos)]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<GpsDetail>> SetGpsOverrideAsync(int id, GpsCoordinate gps)
    {
        if(gps == null)
        {
            throw new ArgumentNullException(nameof(gps));
        }

        await _svc.SetGpsOverrideAsync(id, gps, User.GetUsername());

        var detail = await _svc.GetGpsDetailAsync(id, User.GetAllRoles());

        if(detail == null)
        {
            return NotFound();
        }

        return detail;
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
        if(userRating == null)
        {
            throw new ArgumentNullException(nameof(userRating));
        }

        var username = User.GetUsername();

        // TODO: handle invalid photo id?
        // TODO: remove photoId from userPhotoRating?
        if(userRating.Rating < 1)
        {
            await _svc.RemoveRatingAsync(id, username, User.GetAllRoles());
        }
        else if(userRating.Rating <= 5)
        {
            await _svc.SaveRatingAsync(id, username, userRating.Rating, User.GetAllRoles());
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

    async Task<ApiCollectionResult<Comment>> InternalGetCommentsAsync(int id)
    {
        var comments = await _svc.GetCommentsAsync(id, User.GetAllRoles());

        return new ApiCollectionResult<Comment>(comments.ToList());
    }

    Task<Rating?> InternalGetRatingAsync(int id)
    {
        return _svc.GetRatingsAsync(id, User.GetUsername(), User.GetAllRoles());
    }
}
