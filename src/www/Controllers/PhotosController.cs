using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Maw.Domain.Photos;
using MawMvcApp.ViewModels;
using MawMvcApp.ViewModels.Navigation;
using Maw.Security;


namespace MawMvcApp.Controllers
{
	[Authorize(Policy.ViewPhotos)]
    [Route("photos")]
    public class PhotosController
        : MawBaseController<PhotosController>
    {
		const string MOBILE_THUMB_MIME_TYPE = "image/jpeg";
        const string ZIP_MIME_TYPE = "application/zip";
		const int MOBILE_THUMB_SIZE = 60;

        readonly IPhotoService _svc;
        readonly IImageCropper _imageCropper;
        readonly IPhotoZipper _photoZipper;
        readonly IAntiforgery _antiForgery;


		public PhotosController(ILogger<PhotosController> log,
                                IPhotoService photoService,
                                IImageCropper imageCropper,
                                IPhotoZipper photoZipper,
                                IAntiforgery antiForgery)
			: base(log)
        {
			_svc = photoService ?? throw new ArgumentNullException(nameof(photoService));
			_imageCropper = imageCropper ?? throw new ArgumentNullException(nameof(imageCropper));
            _photoZipper = photoZipper ?? throw new ArgumentNullException(nameof(photoZipper));
            _antiForgery = antiForgery ?? throw new ArgumentNullException(nameof(antiForgery));
        }


        [HttpGet("{*extra}")]
        public IActionResult Index()
        {
			ViewBag.NavigationZone = NavigationZone.Photos;

            return View();
        }


        [HttpGet("3d/{*extra}")]
        public IActionResult ThreeD()
        {
            ViewBag.NavigationZone = NavigationZone.Photos;

            return View();
        }


        [HttpGet("stats/{*extra}")]
        public IActionResult Stats()
        {
            ViewBag.NavigationZone = NavigationZone.Photos;

            return View();
        }


        [HttpGet("GetMobileThumbnail/{id:int}")]
        public async Task<IActionResult> GetMobileThumbnail(short id)
		{
            var category = await _svc.GetCategoryAsync(id, Role.IsAdmin(User));
			var thumbInfo = category.TeaserPhotoInfo;
            var croppedImageStream = _imageCropper.CropImage(thumbInfo.Path, MOBILE_THUMB_SIZE);

            if(croppedImageStream == null)
            {
                return NotFound();
            }

            return File(croppedImageStream, MOBILE_THUMB_MIME_TYPE);
		}


        [HttpGet("download-category/{id:int}")]
        public async Task<IActionResult> DownloadCategory(short id)
        {
            var photos = await _svc.GetPhotosForCategoryAsync(id, Role.IsAdmin(User));
            var stream = _photoZipper.Zip(photos);

            if(stream == null)
            {
                return NotFound();
            }

			return File(stream, ZIP_MIME_TYPE, "photos.zip");
        }
    }
}
