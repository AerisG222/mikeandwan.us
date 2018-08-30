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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Maw.Domain.Upload;
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


		public UploadController(ILogger<UploadController> log,
                                IUploadService uploadSvc,
                                IHubContext<UploadHub> uploadHubCtx)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _uploadSvc = uploadSvc ?? throw new ArgumentNullException(nameof(uploadSvc));
            _uploadHub = uploadHubCtx ?? throw new ArgumentNullException(nameof(uploadHubCtx));
        }


        [HttpGet("files")]
        public IActionResult Files()
        {
            return Ok(_uploadSvc.GetFileList(User));
        }


        [HttpPost("upload")]
        [RequestSizeLimit(2_147_483_648)]  // 2GB
        public async Task<IActionResult> Upload(IFormFile file)
        {
            return Ok(await _uploadSvc.SaveFileAsync(User, file.FileName, file.OpenReadStream()));
        }


        [HttpPost("delete")]
        public IActionResult Delete(string[] relativePaths)
        {
            return Ok(_uploadSvc.DeleteFiles(User, relativePaths));
        }


        [HttpGet("download")]
        public IActionResult Download(string[] downloadFiles)
        {
            return BadRequest();
        }
    }
}
