using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Maw.Domain.Models.Blogs;
using Maw.Domain.Blogs;
using Maw.Domain.Photos;
using Maw.Domain.Videos;
using MawMvcApp.ViewModels.Admin;
using MawMvcApp.ViewModels.Navigation;
using Maw.Security;

namespace MawMvcApp.Controllers;

[Authorize(MawPolicy.AdminSite)]
[Route("admin")]
public class AdminController
    : MawBaseController<AdminController>
{
    readonly IBlogService _blogSvc;
    readonly IPhotoService _photoSvc;
    readonly IVideoService _videoSvc;

    public AdminController(
        ILogger<AdminController> log,
        IBlogService blogService,
        IPhotoService photoService,
        IVideoService videoService)
        : base(log)
    {
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
        ArgumentNullException.ThrowIfNull(model);

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

                await _blogSvc.AddPostAsync(post);

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
}
