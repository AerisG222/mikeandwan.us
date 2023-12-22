using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Maw.Domain.Photos;
using Maw.Security;

namespace MawMvcApp.Controllers;

[Authorize(MawPolicy.ViewPhotos)]
[Route("photos")]
public class PhotosController
    : MawBaseController<PhotosController>
{
    const int MOBILE_THUMB_SIZE = 60;

    readonly IPhotoService _svc;
    readonly IImageCropper _imageCropper;
    readonly IPhotoZipper _photoZipper;
    readonly IFileProvider _fileProvider;
    readonly IContentTypeProvider _contentTypeProvider;

    public PhotosController(
        ILogger<PhotosController> log,
        IPhotoService photoService,
        IImageCropper imageCropper,
        IPhotoZipper photoZipper,
        IFileProvider fileProvider,
        IContentTypeProvider contentTypeProvider)
        : base(log)
    {
        ArgumentNullException.ThrowIfNull(photoService);
        ArgumentNullException.ThrowIfNull(imageCropper);
        ArgumentNullException.ThrowIfNull(photoZipper);
        ArgumentNullException.ThrowIfNull(fileProvider);
        ArgumentNullException.ThrowIfNull(contentTypeProvider);

        _svc = photoService;
        _imageCropper = imageCropper;
        _photoZipper = photoZipper;
        _fileProvider = fileProvider;
        _contentTypeProvider = contentTypeProvider;
    }

    [HttpGet("{*extra}")]
    public IActionResult Index()
    {
        return Redirect("https://photos.mikeandwan.us");
    }

    [HttpGet("GetMobileThumbnail/{id:int}")]
    public async Task<IActionResult> GetMobileThumbnail(short id)
    {
        var category = await _svc.GetCategoryAsync(id, User.GetAllRoles());

        if(category == null)
        {
            return NotFound();
        }

        var thumbInfo = category.TeaserImage;
        var croppedImageStream = _imageCropper.CropImage(thumbInfo.Path, MOBILE_THUMB_SIZE);

        if (croppedImageStream == null)
        {
            return NotFound();
        }

        return File(croppedImageStream, GetContentType(thumbInfo.Path));
    }

    [HttpGet("download-category/{id:int}")]
    public async Task<IActionResult> DownloadCategory(short id)
    {
        var filename = "photos.zip";
        var photos = await _svc.GetPhotosForCategoryAsync(id, User.GetAllRoles());
        var stream = _photoZipper.Zip(photos);

        if (stream == null)
        {
            return NotFound();
        }

        return File(stream, GetContentType(filename), filename);
    }

#pragma warning disable CA2000
    [HttpGet("download/{id:int}/{size:length(2,5)}")]
    public async Task<IActionResult> Download(int id, string size)
    {
        Log.LogDebug("Attempting to download photo with id: {PhotoId} and size: {Size}", id, size);

        string path;
        var photo = await _svc.GetPhotoAsync(id, User.GetAllRoles());

        if(photo == null)
        {
            return NotFound();
        }

        switch (size?.ToLower(CultureInfo.InvariantCulture))
        {
            case "xs":
                path = photo.XsInfo.Path;
                break;
            case "xs_sq":
                path = photo.XsSqInfo.Path;
                break;
            case "sm":
                path = photo.SmInfo.Path;
                break;
            case "md":
                path = photo.MdInfo.Path;
                break;
            case "lg":
                path = photo.LgInfo.Path;
                break;
            case "prt":
                path = photo.PrtInfo.Path;
                break;
            default:
                return BadRequest();
        }

        var fi = _fileProvider.GetFileInfo(path);

        if (!fi.Exists || string.IsNullOrEmpty(fi.PhysicalPath))
        {
            return BadRequest();
        }

#pragma warning disable SCS0018
        var stream = new FileStream(fi.PhysicalPath, FileMode.Open, FileAccess.Read);
#pragma warning restore SCS0018

        return File(stream, GetContentType(path), Path.GetFileName(path));
    }
#pragma warning restore CA2000

    string GetContentType(string filename)
    {
        if (_contentTypeProvider.TryGetContentType(filename, out var contentType))
        {
            return contentType;
        }

        return "application/octet-stream";
    }
}
