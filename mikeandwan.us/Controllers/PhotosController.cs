using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using NMagickWand;
using Maw.Domain.Photos;
using MawMvcApp.Filters;
using MawMvcApp.ViewModels;
using MawMvcApp.ViewModels.Navigation;


namespace MawMvcApp.Controllers
{
	[Authorize(MawConstants.POLICY_VIEW_PHOTOS)]
    [Route("photos")]
    public class PhotosController
        : MawBaseController<PhotosController>
    {
		const int TWENTY_MB = 20 * 1024 * 1024;
		const string MOBILE_THUMB_MIME_TYPE = "image/jpeg";
        const string ZIP_MIME_TYPE = "application/zip";
		const int MOBILE_THUMB_SIZE = 60;

        readonly PhotoService _svc;
        readonly IFileProvider _fileProvider;
        readonly IAntiforgery _antiForgery;


		public PhotosController(IAuthorizationService authorizationService,
                                ILogger<PhotosController> log,
                                IPhotoRepository photoRepository,
                                IFileProvider fileProvider,
                                IAntiforgery antiForgery)
			: base(authorizationService, log)
        {
			if(photoRepository == null)
			{
				throw new ArgumentNullException(nameof(photoRepository));
			}

			if(fileProvider == null)
			{
				throw new ArgumentNullException(nameof(fileProvider));
			}

            if (antiForgery == null)
            {
                throw new ArgumentNullException(nameof(antiForgery));
            }

			_svc = new PhotoService(photoRepository);
			_fileProvider = fileProvider;
            _antiForgery = antiForgery;
        }


        [HttpGet("{*extra}")]
        [TypeFilter(typeof(ApiAntiforgeryActionFilter))]
        public ActionResult Index()
        {
			ViewBag.NavigationZone = NavigationZone.Photos;

            return View();
        }


        [HttpGet("GetMobileThumbnail/{id:int}")]
        public async Task<ActionResult> GetMobileThumbnail(short id)
		{
            var category = await _svc.GetCategoryAsync(id, User.IsInRole(MawConstants.ROLE_ADMIN));
			var thumbInfo = category.TeaserPhotoInfo;
            var fi = _fileProvider.GetFileInfo(thumbInfo.Path);

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


        [HttpGet("download-category/{id:int}")]
        public async Task<FileResult> DownloadCategory(short id)
        {
            var photos = await _svc.GetPhotosForCategoryAsync(id, User.IsInRole(MawConstants.ROLE_ADMIN));
			var ms = new MemoryStream(TWENTY_MB);

            using(var za = new ZipArchive(ms, ZipArchiveMode.Create, true))
            {
                foreach(var photo in photos)
                {
					var path = _fileProvider.GetFileInfo(photo.LgInfo.Path).PhysicalPath;

					_log.LogInformation(string.Format("Adding file {0} to archive", path));

					za.CreateEntryFromFile(path, Path.GetFileName(path));
                }
            }

			ms.Seek(0, SeekOrigin.Begin);

			return File(ms, ZIP_MIME_TYPE, "photos.zip");
        }
    }
}
