using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Maw.Domain.Email;
using Maw.Domain.Identity;
using MawAuth.ViewModels.Account;
using SignInRes = Microsoft.AspNetCore.Identity.SignInResult;
using Microsoft.AspNetCore.Authentication;
using Duende.IdentityServer.Services;
using MawAuth.ViewModels.Email;
using Mvc.RenderViewToString;

namespace MawAuth.Controllers;

[Route("account")]
public class AccountController
    : Controller
{
    const byte LOGIN_AREA_FORM = 1;
    const string EmailFrom = "webmaster@mikeandwan.us";

    readonly ILogger _log;
    readonly IIdentityServerInteractionService _interaction;
    readonly IUserRepository _repo;
    readonly SignInManager<MawUser> _signInManager;
    readonly UserManager<MawUser> _userMgr;
    readonly ILoginService _loginService;
    readonly RazorViewToStringRenderer _razorRenderer;
    readonly IEmailService _emailService;

    public AccountController(
        ILogger<AccountController> log,
        IIdentityServerInteractionService interaction,
        IUserRepository userRepository,
        SignInManager<MawUser> signInManager,
        UserManager<MawUser> userManager,
        ILoginService loginService,
        IEmailService emailService,
        RazorViewToStringRenderer razorRenderer)
    {
        _log = log ?? throw new ArgumentNullException(nameof(log));
        _interaction = interaction ?? throw new ArgumentNullException(nameof(interaction));
        _repo = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
        _userMgr = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _loginService = loginService ?? throw new ArgumentNullException(nameof(loginService));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _razorRenderer = razorRenderer ?? throw new ArgumentNullException(nameof(razorRenderer));
    }

    [HttpGet("login")]
    public async Task<IActionResult> Login(string returnUrl)
    {
        var vm = new LoginModel()
        {
            ReturnUrl = returnUrl,
            ExternalSchemes = await GetExternalLoginSchemes().ConfigureAwait(false)
        };

        return View(vm);
    }

    [HttpPost("login")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginModel model)
    {
        if (model == null)
        {
            throw new ArgumentNullException(nameof(model));
        }

        model.WasAttempted = true;
        model.ExternalSchemes = await GetExternalLoginSchemes().ConfigureAwait(false);

        if (!ModelState.IsValid)
        {
            LogValidationErrors();

            return View(model);
        }

        var result = await _loginService.AuthenticateAsync(model.Username, model.Password, LOGIN_AREA_FORM).ConfigureAwait(false);

        _log.LogInformation("Login complete");

        if (result == SignInRes.Success)
        {
            return RedirectToLocal(model.ReturnUrl);
        }
        else
        {
            ModelState.AddModelError("Error", "Sorry, a user was not found with this username/password combination");
        }

        return View(model);
    }

    [HttpGet("external-login")]
    public IActionResult ExternalLogin(string provider, string returnUrl)
    {
        var props = new AuthenticationProperties()
        {
            RedirectUri = Url.Action(nameof(ExternalLoginCallback)),
            Items =
            {
                { "returnUrl", returnUrl },
                { "scheme", provider },
            }
        };

        return Challenge(props, provider);
    }

    [HttpGet("external-login-callback")]
    public async Task<IActionResult> ExternalLoginCallback()
    {
        var result = await HttpContext.AuthenticateAsync(Duende.IdentityServer.IdentityServerConstants.ExternalCookieAuthenticationScheme).ConfigureAwait(false);
        var items = result?.Properties?.Items;

        if (result?.Succeeded != true || items == null || !items.ContainsKey("scheme"))
        {
            _log.LogError("Unable to obtain external login info");
            return View();
        }

        var provider = items["scheme"];
        var email = result.Principal?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email);

        if (email == null)
        {
            _log.LogError("Unable to obtain email from External Authentication Provider {Provider}", provider);
            return View();
        }

        var user = await _userMgr.FindByEmailAsync(email.Value).ConfigureAwait(false);

        if (user != null)
        {
            if (user.IsExternalAuthEnabled(provider))
            {
                // now sign in the local user
                await _signInManager.SignInAsync(user, false).ConfigureAwait(false);
                await _loginService.LogExternalLoginAttemptAsync(email.Value, provider, true).ConfigureAwait(false);

                // delete temporary cookie used during external authentication
                await HttpContext.SignOutAsync(Duende.IdentityServer.IdentityServerConstants.ExternalCookieAuthenticationScheme).ConfigureAwait(false);

                _log.LogInformation("User {Username} logged in with {Provider} provider.", user.Username, provider);

                // validate return URL and redirect back to authorization endpoint or a local page
                var returnUrl = result.Properties.Items["returnUrl"];

                if (_interaction.IsValidReturnUrl(returnUrl) || Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    // we should have a valid redirect url, but if they login and we don't,
                    // let them review there profile...
                    return RedirectToAction(nameof(EditProfile));
                }
            }
            else
            {
                _log.LogError("User {Username} unable to login with {Provider} as they have not yet opted-in for this provider.", user.Username, provider);
            }
        }

        await _loginService.LogExternalLoginAttemptAsync(email.Value, provider, false).ConfigureAwait(false);

        return View();
    }

    [HttpGet("logout")]
    public async Task<IActionResult> Logout(string logoutId)
    {
        var logout = await _interaction.GetLogoutContextAsync(logoutId).ConfigureAwait(false);

        await _signInManager.SignOutAsync().ConfigureAwait(false);

#pragma warning disable SCS0027
        return Redirect(logout.PostLogoutRedirectUri);
#pragma warning restore SCS0027
    }

    [HttpGet("forgot-password")]
    public IActionResult ForgotPassword()
    {
        return View(new ForgotPasswordModel());
    }

    [HttpPost("forgot-password")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
    {
        if (model == null)
        {
            throw new ArgumentNullException(nameof(model));
        }

        model.WasEmailAttempted = true;

        if (ModelState.IsValid)
        {
            var user = await _userMgr.FindByEmailAsync(model.Email).ConfigureAwait(false);

            if (user == null)
            {
                _log.LogInformation("Unable to find user with email [{Email}].", model.Email);

                return View(model);
            }

            // legacy users might not have the security stamp set.  if so, set it here, as a non-null security stamp is required for this to work
            if (string.IsNullOrEmpty(user.SecurityStamp))
            {
                await _userMgr.UpdateSecurityStampAsync(user).ConfigureAwait(false);
            }

            var code = await _userMgr.GeneratePasswordResetTokenAsync(user).ConfigureAwait(false);
            var callbackUrl = Url.Action("ResetPassword", "Account", new { user.Email, code }, Request.Scheme);

            _log.LogInformation("Sending password reset email to user: {User}", user.Name);

            var emailModel = new ResetPasswordEmailModel
            {
                Title = "Reset Password",
                CallbackUrl = callbackUrl
            };

            var body = await _razorRenderer.RenderViewToStringAsync("~/Views/Email/ResetPassword.cshtml", emailModel).ConfigureAwait(false);

            await _emailService.SendHtmlAsync(user.Email, EmailFrom, "Reset Password for mikeandwan.us", body).ConfigureAwait(false);

            model.WasSuccessful = true;

            return View(model);
        }
        else
        {
            LogValidationErrors();
        }

        return View(model);
    }

    [HttpGet("reset-password")]
    public async Task<IActionResult> ResetPassword(string code)
    {
        var model = new ResetPasswordModel();

        await TryUpdateModelAsync<ResetPasswordModel>(model, string.Empty, x => x.Email, x => x.Code).ConfigureAwait(false);
        ModelState.Clear();

        return View(model);
    }

    [HttpPost("reset-password")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
    {
        if (model == null)
        {
            throw new ArgumentNullException(nameof(model));
        }

        model.ResetAttempted = true;

        if (ModelState.IsValid)
        {
            var user = await _repo.GetUserByEmailAsync(model.Email).ConfigureAwait(false);
            var result = await _userMgr.ResetPasswordAsync(user, model.Code, model.NewPassword).ConfigureAwait(false);

            if (result.Succeeded)
            {
                model.WasReset = true;
            }
            else
            {
                _log.LogWarning("reset password result: {Result}", result.ToString());

                AddErrors(result);
            }
        }
        else
        {
            LogValidationErrors();
        }

        return View(model);
    }

    [Authorize]
    [HttpGet("change-password")]
    public IActionResult ChangePassword()
    {
        var m = new ChangePasswordModel();

        return View(m);
    }

    [HttpPost("change-password")]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
    {
        if (model == null)
        {
            throw new ArgumentNullException(nameof(model));
        }

        model.ChangeAttempted = true;

        if (ModelState.IsValid)
        {
            var user = await _repo.GetUserAsync(User.Identity.Name).ConfigureAwait(false);
            var result = await _userMgr.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword).ConfigureAwait(false);

            if (result.Succeeded)
            {
                model.ChangeSucceeded = true;
            }
            else
            {
                _log.LogWarning("change password result: {Result}", result.ToString());

                AddErrors(result);
            }
        }
        else
        {
            LogValidationErrors();
        }

        return View(model);
    }

    [HttpGet("access-denied")]
    public IActionResult AccessDenied()
    {
        return View();
    }

    [Authorize]
    [HttpGet("edit-profile")]
    public async Task<IActionResult> EditProfile()
    {
        ViewBag.States = await GetStateSelectListItemsAsync().ConfigureAwait(false);
        ViewBag.Countries = await GetCountrySelectListItemsAsync().ConfigureAwait(false);

        var user = await _userMgr.FindByNameAsync(User.Identity.Name).ConfigureAwait(false);

        var model = new ProfileModel
        {
            Username = user.Username,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            EnableGithubAuth = user.IsGithubAuthEnabled,
            EnableGoogleAuth = user.IsGoogleAuthEnabled,
            EnableMicrosoftAuth = user.IsMicrosoftAuthEnabled,
            EnableTwitterAuth = user.IsTwitterAuthEnabled
        };

        return View(model);
    }

    [HttpPost("edit-profile")]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProfile(ProfileModel model)
    {
        if (model == null)
        {
            throw new ArgumentNullException(nameof(model));
        }

        ViewBag.States = await GetStateSelectListItemsAsync().ConfigureAwait(false);
        ViewBag.Countries = await GetCountrySelectListItemsAsync().ConfigureAwait(false);

        model.WasAttempted = true;
        model.Username = User.Identity.Name;

        if (ModelState.IsValid)
        {
            var user = await _userMgr.FindByNameAsync(User.Identity.Name).ConfigureAwait(false);

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;
            user.IsGithubAuthEnabled = model.EnableGithubAuth;
            user.IsGoogleAuthEnabled = model.EnableGoogleAuth;
            user.IsMicrosoftAuthEnabled = model.EnableMicrosoftAuth;
            user.IsTwitterAuthEnabled = model.EnableTwitterAuth;

            await _repo.UpdateUserAsync(user).ConfigureAwait(false);

            model.WasUpdated = true;
        }
        else
        {
            LogValidationErrors();
        }

        return View(model);
    }

    async Task<IEnumerable<SelectListItem>> GetStateSelectListItemsAsync()
    {
        var states = await _repo.GetStatesAsync().ConfigureAwait(false);

        return states.Select(x => new SelectListItem
        {
            Value = x.Code,
            Text = x.Name
        });
    }

    async Task<IEnumerable<SelectListItem>> GetCountrySelectListItemsAsync()
    {
        var countries = await _repo.GetCountriesAsync().ConfigureAwait(false);

        return countries.Select(x => new SelectListItem
        {
            Value = x.Code,
            Text = x.Name
        });
    }

    async Task<IEnumerable<ExternalLoginScheme>> GetExternalLoginSchemes()
    {
        var schemes = await _signInManager.GetExternalAuthenticationSchemesAsync().ConfigureAwait(false);

        return schemes
            .Select(x => new ExternalLoginScheme(x))
            .OrderBy(x => x.ExternalAuth.DisplayName);
    }

    void AddErrors(IdentityResult result)
    {
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
    }

    IActionResult RedirectToLocal(string returnUrl)
    {
        if (Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }
        else
        {
            return Redirect("/");
            //return RedirectToAction(nameof(HomeController.Index), "Home");
        }
    }

    void LogValidationErrors()
    {
        var errs = ModelState.Values.SelectMany(v => v.Errors);

        foreach (var err in errs)
        {
            _log.LogWarning("validation error: {ValidationError}", err.ErrorMessage);
        }
    }
}
