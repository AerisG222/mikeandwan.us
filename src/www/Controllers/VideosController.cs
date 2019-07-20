using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Maw.Domain.Videos;
using Maw.Domain.Photos;
using Maw.Security;


namespace MawMvcApp.Controllers
{
	[Authorize(Policy.ViewVideos)]
    [Route("videos")]
    public class VideosController
        : MawBaseController<VideosController>
    {
        const string MOBILE_THUMB_MIME_TYPE = "image/png";
        const int MOBILE_THUMB_SIZE = 60;


        readonly IVideoService _svc;
        readonly IImageCropper _imageCropper;


		public VideosController(ILogger<VideosController> log,
                                IVideoService videoService,
                                IImageCropper imageCropper)
			: base(log)
        {
            _svc = videoService ?? throw new ArgumentNullException(nameof(videoService));
			_imageCropper = imageCropper ?? throw new ArgumentNullException(nameof(imageCropper));;
        }


        [HttpGet("{*extra}")]
        public IActionResult Index()
        {
			return Redirect("https://videos.mikeandwan.us");
        }


        [HttpGet("GetMobileCategoryThumbnail/{id:int}")]
        public async Task<IActionResult> GetMobileCategoryThumbnail(short id)
        {
            var category = await _svc.GetCategoryAsync(id, Role.IsAdmin(User)).ConfigureAwait(false);

            return GetScaledImage(category.TeaserImage.Path);
        }


        [HttpGet("GetMobileVideoThumbnail/{id:int}")]
        public async Task<IActionResult> GetMobileVideoThumbnail(short id)
        {
            var video = await _svc.GetVideoAsync(id, Role.IsAdmin(User)).ConfigureAwait(false);

            return GetScaledImage(video.Thumbnail.Path);
        }


        IActionResult GetScaledImage(string path)
        {
            var croppedImageStream = _imageCropper.CropImage(path, MOBILE_THUMB_SIZE);

            if(croppedImageStream == null)
            {
                return NotFound();
            }

            return File(croppedImageStream, MOBILE_THUMB_MIME_TYPE);
        }
    }
}
