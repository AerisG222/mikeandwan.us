﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Maw.Domain;
using Maw.Domain.Models;
using Maw.Domain.Videos;
using Maw.Security;
using MawApi.Models.Videos;
using MawApi.Services.Videos;
using MawApi.ViewModels;

namespace MawApi.Controllers;

[ApiController]
[Authorize]
[Authorize(MawPolicy.ViewVideos)]
[Route("videos")]
public class VideosController
    : ControllerBase
{
    readonly IVideoService _svc;
    readonly VideoAdapter _adapter;

    public VideosController(
        VideoAdapter videoAdapter,
        IVideoService videoService)
    {
        ArgumentNullException.ThrowIfNull(videoService);
        ArgumentNullException.ThrowIfNull(videoAdapter);

        _svc = videoService;
        _adapter = videoAdapter;
    }

    [HttpGet("{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<MawApi.ViewModels.Videos.VideoViewModel>> GetByIdAsync(short id)
    {
        var video = await _svc.GetVideoAsync(id, User.GetAllRoles());

        if (video == null)
        {
            return NotFound();
        }

        return _adapter.Adapt(video);
    }

    [HttpGet("{id}/comments")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public Task<ApiCollectionResult<Comment>> GetCommentsAsync(short id)
    {
        return InternalGetCommentsAsync(id);
    }

    [HttpPost("{id}/comments")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<ApiCollectionResult<Comment>> AddCommentAsync(short id, CommentViewModel model)
    {
        if (model?.Comment == null)
        {
            throw new ArgumentNullException(nameof(model));
        }

        // TODO: handle invalid photo id?
        // TODO: remove photoId from commentViewModel?
        await _svc.InsertCommentAsync(id, User.GetUsername(), model.Comment, User.GetAllRoles());

        return await InternalGetCommentsAsync(id);
    }

    [HttpGet("{id}/gps")]
    [Authorize(MawPolicy.AdminVideos)]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<GpsDetail>> GetGpsDetailAsync(short id)
    {
        var gps = await _svc.GetGpsDetailAsync(id, User.GetAllRoles());

        if (gps == null)
        {
            return NotFound();
        }

        return gps;
    }

    [HttpPatch("{id}/gps")]
    [Authorize(MawPolicy.AdminVideos)]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<GpsDetail>> SetGpsOverrideAsync(short id, GpsCoordinate gps)
    {
        ArgumentNullException.ThrowIfNull(gps);

        await _svc.SetGpsOverrideAsync(id, gps, User.GetUsername(), User.GetAllRoles());

        var detail = await _svc.GetGpsDetailAsync(id, User.GetAllRoles());

        if (detail == null)
        {
            return NotFound();
        }

        return detail;
    }

    [HttpGet("{id}/rating")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<Rating>> GetRatingAsync(short id)
    {
        var rating = await InternalGetRatingAsync(id);

        if (rating == null)
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
    public async Task<ActionResult<Rating>> RateVideoAsync(short id, UserRating userRating)
    {
        ArgumentNullException.ThrowIfNull(userRating);

        var username = User.GetUsername();

        // TODO: handle invalid photo id?
        // TODO: remove photoId from userPhotoRating?
        if (userRating.Rating < 1)
        {
            await _svc.RemoveRatingAsync(id, username, User.GetAllRoles());
        }
        else if (userRating.Rating <= 5)
        {
            await _svc.SaveRatingAsync(id, username, userRating.Rating, User.GetAllRoles());
        }
        else
        {
            return BadRequest();
        }

        var rating = await InternalGetRatingAsync(id);

        if (rating == null)
        {
            return NotFound();
        }

        return rating;
    }

    async Task<ApiCollectionResult<Comment>> InternalGetCommentsAsync(short id)
    {
        var comments = await _svc.GetCommentsAsync(id, User.GetAllRoles());

        return new ApiCollectionResult<Comment>(comments.ToList());
    }

    Task<Rating?> InternalGetRatingAsync(short id)
    {
        return _svc.GetRatingsAsync(id, User.GetUsername(), User.GetAllRoles());
    }
}
