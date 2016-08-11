using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Maw.Domain.Videos;
using MawMvcApp.Filters;
using MawMvcApp.ViewModels.Navigation;
using MawMvcApp.ViewModels;
using NMagickWand;


namespace MawMvcApp.Controllers
{
	[Authorize(MawConstants.POLICY_VIEW_VIDEOS)]
    [Route("videos")]
    public class VideosController
        : MawBaseController<VideosController>
    {
        const string MOBILE_THUMB_MIME_TYPE = "image/png";
        const int MOBILE_THUMB_SIZE = 60;


        readonly VideoService _svc;
        readonly IFileProvider _fileProvider;


		public VideosController(IAuthorizationService authorizationService,
                                ILogger<VideosController> log,
                                IVideoRepository videoRepository,
                                IFileProvider fileProvider)
			: base(authorizationService, log)
        {
			if(videoRepository == null)
			{
				throw new ArgumentNullException(nameof(videoRepository));
			}

			if(fileProvider == null)
			{
				throw new ArgumentNullException(nameof(fileProvider));
			}

            _svc = new VideoService(videoRepository);
			_fileProvider = fileProvider;
        }


        [HttpGet("{*extra}")]
        [TypeFilter(typeof(ApiAntiforgeryActionFilter))]
        public ActionResult Index()
        {
			ViewBag.NavigationZone = NavigationZone.Videos;

            return View();
        }


        [HttpGet("GetMobileCategoryThumbnail/{id:int}")]
        public async Task<ActionResult> GetMobileCategoryThumbnail(short id)
        {
            var category = await _svc.GetCategoryAsync(id, User.IsInRole(MawConstants.ROLE_ADMIN));

            return GetScaledImage(category.TeaserThumbnail.Path, category.TeaserThumbnail.Width, category.TeaserThumbnail.Height);
        }


        [HttpGet("GetMobileVideoThumbnail/{id:int}")]
        public async Task<ActionResult> GetMobileVideoThumbnail(short id)
        {
            var video = await _svc.GetVideoAsync(id, User.IsInRole (MawConstants.ROLE_ADMIN));

            return GetScaledImage(video.ThumbnailVideo.Path, video.ThumbnailVideo.Width, video.ThumbnailVideo.Height);
        }


        ActionResult GetScaledImage(string path, int width, int height)
        {
            var fi = _fileProvider.GetFileInfo(path);

            if(fi.Exists)
            {
                using(var mw = new MagickWand(fi.PhysicalPath))
                {
                    var leftPos = (mw.ImageWidth / 2) - (MOBILE_THUMB_SIZE / 2);
                    var topPos = (mw.ImageHeight / 2) - (MOBILE_THUMB_SIZE / 2);

                    var ms = new MemoryStream();

                    mw.CropImage(MOBILE_THUMB_SIZE, MOBILE_THUMB_SIZE, (int)leftPos, (int)topPos);
                    mw.WriteImage(ms);

                    ms.Seek(0, SeekOrigin.Begin);

                    return File(ms, MOBILE_THUMB_MIME_TYPE);
                }
            }

            return NotFound();
        }
    }
}
