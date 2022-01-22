using System;
using System.Threading.Tasks;
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
using MawMvcApp.ViewModels.Email;
using Mvc.RenderViewToString;

namespace MawMvcApp.Controllers;

[Route("about")]
public class AboutController
    : MawBaseController<AboutController>
{
    readonly ContactConfig _config;
    readonly ICaptchaService _captchaService;
    readonly IBlogService _blogService;
    readonly IEmailService _emailService;
    readonly RazorViewToStringRenderer _razorRenderer;

    public AboutController(
        ILogger<AboutController> log,
        IOptions<ContactConfig> contactOpts,
        IBlogService blogService,
        ICaptchaService captchaService,
        IEmailService emailService,
        RazorViewToStringRenderer razorRenderer)
        : base(log)
    {
        if (contactOpts == null)
        {
            throw new ArgumentNullException(nameof(contactOpts));
        }

        _config = contactOpts.Value;

        _blogService = blogService ?? throw new ArgumentNullException(nameof(blogService));
        _captchaService = captchaService ?? throw new ArgumentNullException(nameof(captchaService));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _razorRenderer = razorRenderer ?? throw new ArgumentNullException(nameof(razorRenderer));
    }

    [HttpGet("")]
    public IActionResult Index()
    {
        ViewBag.NavigationZone = NavigationZone.About;

        return View();
    }

    [HttpGet("contact")]
    public IActionResult Contact()
    {
        ViewBag.NavigationZone = NavigationZone.About;

        var model = new ContactModel
        {
            RecaptchaSiteKey = _captchaService.SiteKey
        };

        return View(model);
    }

    [HttpPost("contact")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Contact(IFormCollection collection)
    {
        if (collection == null)
        {
            throw new ArgumentNullException(nameof(collection));
        }

        ViewBag.NavigationZone = NavigationZone.About;

        var model = new ContactModel();
        await TryUpdateModelAsync<ContactModel>(model);
        model.RecaptchaSiteKey = _captchaService.SiteKey;
        model.SubmitAttempted = true;

        if (ModelState.IsValid)
        {
            model.IsHuman = await _captchaService.VerifyAsync(collection["g-recaptcha-response"]);

            if (!model.IsHuman)
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

                    var emailModel = new ContactUsEmailModel
                    (
                        _config.Subject,
                        model.Email,
                        model.FirstName,
                        model.LastName,
                        model.Message
                    );

                    var body = await _razorRenderer.RenderViewToStringAsync("~/Views/Email/ContactUs.cshtml", emailModel);

                    await _emailService.SendHtmlAsync(to, from, subject, body);

                    model.SubmitSuccess = true;
                }
                catch (Exception ex)
                {
                    Log.LogError(ex, "There was an error sending an email: {ErrorMessage}", ex.Message);

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
    public async Task<IActionResult> News()
    {
        ViewBag.NavigationZone = NavigationZone.About;
        var blogs = await _blogService.GetLatestPostsAsync(MawConstants.MawBlogId, 10);

        return View(blogs);
    }
}
