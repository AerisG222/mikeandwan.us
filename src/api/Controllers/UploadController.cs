using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;
using Maw.Domain.Upload;
using Maw.Domain.Utilities;
using Maw.Security;
using MawApi.Hubs;

namespace MawMvcApp.Controllers;

[ApiController]
[Authorize]
[Authorize(MawPolicy.CanUpload)]
[Route("upload")]
public class UploadController
    : Controller
{
    readonly ILogger _log;
    readonly IUploadService _uploadSvc;
    readonly IHubContext<UploadHub> _uploadHub;
    readonly IContentTypeProvider _contentTypeProvider;
    readonly LinuxFileTypeIdentifier _linuxFileTypeIdentifier;

    public UploadController(ILogger<UploadController> log,
                            IUploadService uploadSvc,
                            IHubContext<UploadHub> uploadHubCtx,
                            IContentTypeProvider contentTypeProvider,
                            LinuxFileTypeIdentifier linuxFileTypeIdentifier)
    {
        _log = log ?? throw new ArgumentNullException(nameof(log));
        _uploadSvc = uploadSvc ?? throw new ArgumentNullException(nameof(uploadSvc));
        _uploadHub = uploadHubCtx ?? throw new ArgumentNullException(nameof(uploadHubCtx));
        _contentTypeProvider = contentTypeProvider ?? throw new ArgumentNullException(nameof(contentTypeProvider));
        _linuxFileTypeIdentifier = linuxFileTypeIdentifier ?? throw new ArgumentNullException(nameof(linuxFileTypeIdentifier));
    }

    [HttpGet("files")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    public IActionResult Files()
    {
        return Ok(_uploadSvc.GetFileList(User));
    }

    [HttpPost("upload")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    [RequestSizeLimit(2_147_483_648)]  // 2GB
    public async Task<IActionResult> UploadAsync(IFormFile file)
    {
        if (file == null)
        {
            throw new ArgumentNullException(nameof(file));
        }

        var result = await _uploadSvc.SaveFileAsync(User, file.FileName, file.OpenReadStream()).ConfigureAwait(false);

        if (result.WasSuccessful)
        {
            await UploadHub.FileAddedAsync(_uploadHub, User, result.UploadedFile).ConfigureAwait(false);
        }

        return Ok(result);
    }

    [HttpPost("delete")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> DeleteAsync(string[] relativePaths)
    {
        var results = _uploadSvc.DeleteFiles(User, relativePaths);

        foreach (var result in results)
        {
            if (result.WasSuccessful)
            {
                await UploadHub.FileDeletedAsync(_uploadHub, User, result.UploadedFile).ConfigureAwait(false);
            }
        }

        return Ok(results);
    }

    [HttpPost("download")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public IActionResult Download([FromBody] string[] downloadFiles)
    {
        if (downloadFiles == null)
        {
            throw new ArgumentNullException(nameof(downloadFiles));
        }

        if (downloadFiles.Length == 0)
        {
            return BadRequest();
        }

        Stream stream;
        string filename;
        string fullPath = null;

        try
        {
            if (downloadFiles.Length == 1)
            {
                stream = _uploadSvc.GetFile(User, downloadFiles[0]);
                filename = Path.GetFileName(downloadFiles[0]);
                fullPath = _uploadSvc.GetAbsoluteFilePath(User, downloadFiles[0]);
            }
            else
            {
                stream = _uploadSvc.GetFiles(User, downloadFiles);
                filename = "download.zip";
            }

            return File(stream, GetContentType(fullPath ?? filename), filename);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "There was an error trying to download.");

            return BadRequest();
        }
    }

    [HttpGet("thumbnail/{relativePath}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public IActionResult Thumbnail(string relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
        {
            return BadRequest();
        }

        relativePath = WebUtility.UrlDecode(relativePath);

        var type = GetContentType(relativePath);

        if (type.StartsWith("image", StringComparison.OrdinalIgnoreCase))
        {
            _log.LogDebug("GOT HERE");

            var stream = _uploadSvc.GetThumbnail(User, relativePath, 48);

            _log.LogDebug("is stream null: {StreamIsNull}", stream == null);

            if (stream != null)
            {
                return File(stream, "image/jpeg");
            }
        }

        return BadRequest();
    }

    string GetContentType(string filePath)
    {
        var mimeType = _linuxFileTypeIdentifier.GetMimeType(filePath);

        if (!string.IsNullOrWhiteSpace(mimeType))
        {
            return mimeType;
        }

        if (_contentTypeProvider.TryGetContentType(filePath, out var contentType))
        {
            return contentType;
        }

        return "application/octet-stream";
    }
}
