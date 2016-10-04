using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Maw.Domain.Blogs;
using Maw.Domain.Captcha;
using Maw.Domain.Email;
using MawMvcApp.ViewModels;
using MawMvcApp.ViewModels.About;
using MawMvcApp.ViewModels.Navigation;


namespace MawMvcApp.Controllers
{
	[Route("about")]
    public class AboutController
        : MawBaseController<AboutController>
    {
		readonly ContactConfig _config;
		readonly ICaptchaService _captchaService;
		readonly BlogService _blogService;
		readonly IEmailService _emailService;


		public AboutController(ILogger<AboutController> log, 
							   IOptions<ContactConfig> contactOpts, 
							   IBlogRepository blogRepository, 
							   ICaptchaService captchaService, 
							   IEmailService emailService)
			: base(log)
        {
			if(contactOpts == null)
			{
				throw new ArgumentNullException(nameof(contactOpts));
			}

			if(blogRepository == null)
			{
				throw new ArgumentNullException(nameof(blogRepository));
			}

			if(captchaService == null)
			{
				throw new ArgumentException(nameof(captchaService));
			}

			if(emailService == null)
			{
				throw new ArgumentException(nameof(emailService));
			}

            _config = contactOpts.Value;
			_blogService = new BlogService(blogRepository);
			_captchaService = captchaService;
			_emailService = emailService;
        }


		[HttpGet("")]
        public ActionResult Index()
        {
			ViewBag.NavigationZone = NavigationZone.About;

            return View();
        }
		
		
		[HttpGet("contact")]
		public ActionResult Contact()
		{
			ViewBag.NavigationZone = NavigationZone.About;
			
            var model = new ContactModel();
			model.RecaptchaSiteKey = _captchaService.SiteKey;

			return View(model);
		}
		
		
		[HttpPost("contact")]
		[ValidateAntiForgeryToken]
        public async Task<ActionResult> Contact(IFormCollection collection)
		{
			ViewBag.NavigationZone = NavigationZone.About;

            var model = new ContactModel();
			await TryUpdateModelAsync<ContactModel>(model);
			model.RecaptchaSiteKey = _captchaService.SiteKey;
			model.SubmitAttempted = true;
						
			if(ModelState.IsValid)
			{
				model.IsHuman = await _captchaService.VerifyAsync(collection["g-recaptcha-response"]);

				if(!model.IsHuman)
				{
					ModelState.AddModelError("IsHuman", "The Captcha was not solved correctly, please try again");
				}
				else
				{
					try
					{
						var to = _config.To;
						var from = _config.To;
						var subject = _config.Subject;
						var body = string.Concat("Contact Us Form Submission\n",
							"\n",
							"First Name: ", model.FirstName, "\n",
							"Last Name: ", model.LastName, "\n",
							"Email: ", model.Email, "\n",
							"Message: \n",
							model.Message);

						await _emailService.SendAsync(to, from, subject, body);
					
						model.SubmitSuccess = true;
					}
					catch(Exception ex)
					{
						_log.LogError("There was an error sending an email: " + ex.Message, ex);
					
						model.SubmitSuccess = false;
					}
				}
			}
			else
			{
				LogValidationErrors();
			}
			
			return View(model);
		}


        [HttpGet("news")]
        public async Task<ActionResult> News()
        {
			ViewBag.NavigationZone = NavigationZone.About;
			var blogs = await _blogService.GetLatestPostsAsync(MawConstants.MAW_BLOG_ID, 10);

            return View(blogs);
        }
    }
}
