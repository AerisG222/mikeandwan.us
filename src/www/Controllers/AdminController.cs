using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Maw.Domain.Blogs;
using Maw.Domain.Email;
using Maw.Domain.Identity;
using Maw.Domain.Utilities;
using MawMvcApp.ViewModels;
using MawMvcApp.ViewModels.Admin;
using MawMvcApp.ViewModels.Email;
using MawMvcApp.ViewModels.Navigation;
using Mvc.RenderViewToString;
using Maw.Security;


namespace MawMvcApp.Controllers
{
	[Authorize(Policy.AdminSite)]
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

			post.Title = date.ToString("MMMM dd, yyyy");
			post.PublishDate = date;

			return View(post);
		}


		[HttpPost("create-blog-post")]
        [ValidateAntiForgeryToken]
		public async Task<IActionResult> CreateBlogPost(BlogPostModel model)
		{
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
}
