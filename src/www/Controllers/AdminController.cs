using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Maw.Domain.Blogs;
using Maw.Domain.Photos;
using Maw.Domain.Videos;
using MawMvcApp.ViewModels.Admin;
using MawMvcApp.ViewModels.Navigation;
using Maw.Security;


namespace MawMvcApp.Controllers
{
    [Authorize(MawPolicy.AdminSite)]
    [Route("admin")]
    public class AdminController
        : MawBaseController<AdminController>
    {
        readonly MawApiService _apiSvc;
        readonly IBlogService _blogSvc;
        readonly IPhotoService _photoSvc;
        readonly IVideoService _videoSvc;


        public AdminController(ILogger<AdminController> log,
                               MawApiService apiService,
                               IBlogService blogService,
                               IPhotoService photoService,
                               IVideoService videoService)
            : base(log)
        {
            _apiSvc = apiService ?? throw new ArgumentNullException(nameof(apiService));
            _blogSvc = blogService ?? throw new ArgumentNullException(nameof(blogService));
            _photoSvc = photoService ?? throw new ArgumentNullException(nameof(photoService));
            _videoSvc = videoService ?? throw new ArgumentNullException(nameof(videoService));
        }


        [HttpGet("")]
        public IActionResult Index()
        {
            ViewBag.NavigationZone = NavigationZone.Administration;

            return View();
        }


        [HttpGet("create-blog-post")]
        public IActionResult CreateBlogPost()
        {
            ViewBag.NavigationZone = NavigationZone.Administration;

            var post = new BlogPostModel();
            var date = DateTime.Now;

            post.Title = date.ToString("MMMM dd, yyyy", CultureInfo.InvariantCulture);
            post.PublishDate = date;

            return View(post);
        }


        [HttpPost("create-blog-post")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBlogPost(BlogPostModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            ViewBag.NavigationZone = NavigationZone.Administration;

            if (ModelState.IsValid)
            {
                if (model.Behavior == BlogPostAction.Preview)
                {
                    model.Preview = true;
                }
                if (model.Behavior == BlogPostAction.Save)
                {
                    model.WasAttempted = true;

                    var post = new Post()
                    {
                        BlogId = 1,
                        Title = model.Title,
                        Description = model.Description,
                        PublishDate = model.PublishDate
                    };

                    await _blogSvc.AddPostAsync(post).ConfigureAwait(false);

                    model.Success = true;
                }
            }
            else
            {
                model.WasAttempted = true;
                LogValidationErrors();
            }

            return View(model);
        }


        [HttpGet("show-request-details")]
        public IActionResult ShowRequestDetails()
        {
            ViewBag.NavigationZone = NavigationZone.Administration;

            return View(HttpContext);
        }


        [HttpGet("clear-photo-cache")]
        public IActionResult ClearPhotoCache()
        {
            ViewBag.NavigationZone = NavigationZone.Administration;

            return View(false);
        }


        [HttpPost("clear-photo-cache")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClearPhotoCache(bool doClear)
        {
            ViewBag.NavigationZone = NavigationZone.Administration;

            await _photoSvc.ClearCacheAsync().ConfigureAwait(false);
            await _apiSvc.ClearPhotoCacheAsync().ConfigureAwait(false);

            return View(true);
        }


        [HttpGet("clear-video-cache")]
        public IActionResult ClearVideoCache()
        {
            ViewBag.NavigationZone = NavigationZone.Administration;

            return View(false);
        }


        [HttpPost("clear-video-cache")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClearVideoCache(bool doClear)
        {
            ViewBag.NavigationZone = NavigationZone.Administration;

            await _videoSvc.ClearCacheAsync().ConfigureAwait(false);
            await _apiSvc.ClearVideoCacheAsync().ConfigureAwait(false);

            return View(true);
        }
    }
}
