using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Maw.Domain.Blogs;
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
		readonly IBlogService _blogSvc;


		public AdminController(ILogger<AdminController> log,
							   IBlogService blogService)
			: base(log)
        {
			_blogSvc = blogService ?? throw new ArgumentNullException(nameof(blogService));
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
            if(model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

			ViewBag.NavigationZone = NavigationZone.Administration;

			if (ModelState.IsValid)
			{
				if(model.Behavior == BlogPostAction.Preview)
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
    }
}
