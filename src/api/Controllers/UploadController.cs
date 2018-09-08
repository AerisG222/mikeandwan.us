using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Maw.Domain.Upload;
using Maw.Domain.Utilities;
using Maw.Security;
using MawApi.Hubs;


namespace MawMvcApp.Controllers
{
    [ApiController]
    [Authorize]
    [Authorize(Policy.CanUpload)]
    [Route("upload")]
    public class UploadController
        : Controller
    {
        readonly ILogger<UploadController> _log;
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
        public IActionResult Files()
        {
            return Ok(_uploadSvc.GetFileList(User));
        }


        [HttpPost("upload")]
        [RequestSizeLimit(2_147_483_648)]  // 2GB
        public async Task<IActionResult> UploadAsync(IFormFile file)
        {
            var result = await _uploadSvc.SaveFileAsync(User, file.FileName, file.OpenReadStream());

            if(result.WasSuccessful)
            {
                await UploadHub.FileAdded(_uploadHub, User, result.UploadedFile);
            }

            return Ok(result);
        }


        [HttpPost("delete")]
        public async Task<IActionResult> DeleteAsync(string[] relativePaths)
        {
            var results = _uploadSvc.DeleteFiles(User, relativePaths);

            foreach(var result in results)
            {
                if(result.WasSuccessful)
                {
                    await UploadHub.FileDeleted(_uploadHub, User, result.UploadedFile);
                }
            }

            return Ok(results);
        }


        [HttpPost("download")]
        public IActionResult Download(string[] downloadFiles)
        {
            if(downloadFiles.Length == 0)
            {
                return BadRequest();
            }

            Stream stream = null;
            string filename = null;
            string fullPath = null;

            try
            {
                if(downloadFiles.Length == 1)
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
            catch(Exception ex)
            {
                _log.LogError(ex, "There was an error trying to download.");

                return BadRequest();
            }
        }


        string GetContentType(string filePath)
        {
            var mimeType = _linuxFileTypeIdentifier.GetMimeType(filePath);

            if(!string.IsNullOrWhiteSpace(mimeType))
            {
                return mimeType;
            }

            if(_contentTypeProvider.TryGetContentType(filePath, out var contentType))
            {
                return contentType;
            }

            return "application/octet-stream";
        }
    }
}
