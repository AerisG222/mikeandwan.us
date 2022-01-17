using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Maw.Domain.Photos;
using Maw.Domain.Videos;
using Maw.Security;

namespace MawApi.Controllers;

[ApiController]
[Authorize]
[Authorize(MawPolicy.AdminSite)]
[Route("admin")]
public class AdminController
    : ControllerBase
{
    readonly IPhotoService _photoSvc;
    readonly IVideoService _videoSvc;

    public AdminController(
        IPhotoService photoService,
        IVideoService videoService)
    {
        _photoSvc = photoService ?? throw new ArgumentNullException(nameof(photoService));
        _videoSvc = videoService ?? throw new ArgumentNullException(nameof(videoService));
    }

    [HttpPost("clear-photo-cache")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    public async Task<bool> ClearPhotoCache()
    {
        await _photoSvc.ClearCacheAsync().ConfigureAwait(false);

        return true;
    }

    [HttpPost("clear-video-cache")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    public async Task<bool> ClearVideoCache()
    {
        await _videoSvc.ClearCacheAsync().ConfigureAwait(false);

        return true;
    }
}
