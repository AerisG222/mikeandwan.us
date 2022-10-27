using Microsoft.AspNetCore.Mvc;
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
    readonly ICaptchaFeature _captchaFeature;
    readonly IBlogService _blogService;
    readonly IEmailService _emailService;
    readonly RazorViewToStringRenderer _razorRenderer;

    public AboutController(
        ILogger<AboutController> log,
        IOptions<ContactConfig> contactOpts,
        IBlogService blogService,
        ICaptchaFeature captchaFeature,
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
        _captchaFeature = captchaFeature ?? throw new ArgumentNullException(nameof(captchaFeature));
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
    public async Task<IActionResult> Contact()
    {
        ViewBag.NavigationZone = NavigationZone.About;

        var captchaService = await _captchaFeature.GetServiceAsync();

        var model = new ContactModel
        {
            RecaptchaSiteKey = captchaService.SiteKey
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
        var captchaService = await _captchaFeature.GetServiceAsync();

        await TryUpdateModelAsync<ContactModel>(model);
        model.RecaptchaSiteKey = captchaService.SiteKey;
        model.SubmitAttempted = true;

        if (ModelState.IsValid)
        {
            var response = collection[captchaService.ResponseFormFieldName];
            model.IsHuman = await captchaService.VerifyAsync(response);

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
