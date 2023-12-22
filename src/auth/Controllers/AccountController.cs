using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Maw.Data.Abstractions;
using Maw.Domain;
using Maw.Domain.Email;
using Maw.Domain.Identity;
using Maw.Domain.Models.Identity;
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
        ArgumentNullException.ThrowIfNull(log);
        ArgumentNullException.ThrowIfNull(interaction);
        ArgumentNullException.ThrowIfNull(userRepository);
        ArgumentNullException.ThrowIfNull(signInManager);
        ArgumentNullException.ThrowIfNull(userManager);
        ArgumentNullException.ThrowIfNull(loginService);
        ArgumentNullException.ThrowIfNull(emailService);
        ArgumentNullException.ThrowIfNull(razorRenderer);

        _log = log;
        _interaction = interaction;
        _repo = userRepository;
        _signInManager = signInManager;
        _userMgr = userManager;
        _loginService = loginService;
        _emailService = emailService;
        _razorRenderer = razorRenderer;
    }

    [HttpGet("login")]
    public async Task<IActionResult> Login(string returnUrl)
    {
        var vm = new LoginModel()
        {
            ReturnUrl = returnUrl,
            ExternalSchemes = await GetExternalLoginSchemes()
        };

        return View(vm);
    }

    [HttpPost("login")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        model.WasAttempted = true;
        model.ExternalSchemes = await GetExternalLoginSchemes();

        if (!ModelState.IsValid)
        {
            LogValidationErrors();

            return View(model);
        }

        var result = await _loginService.AuthenticateAsync(model.Username, model.Password, LOGIN_AREA_FORM);

        _log.LogInformation("Login complete");

        if (result == SignInRes.Success)
        {
            return RedirectToLocal(model.ReturnUrl ?? "/");
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
        var result = await HttpContext.AuthenticateAsync(Duende.IdentityServer.IdentityServerConstants.ExternalCookieAuthenticationScheme);
        var items = result?.Properties?.Items;

        if (result?.Succeeded != true || items == null)
        {
            _log.LogError("Unable to obtain external login info");
            return View();
        }

        items.TryGetValue("scheme", out string? provider);

        if(provider == null)
        {
            _log.LogError("Unable to identify authentication provider");
            return View();
        }

        var email = result.Principal?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email);

        if (email == null)
        {
            _log.LogError("Unable to obtain email from External Authentication Provider {Provider}", provider);
            return View();
        }

        var user = await _userMgr.FindByEmailAsync(email.Value);

        if (user != null)
        {
            if (user.IsExternalAuthEnabled(provider))
            {
                // now sign in the local user
                await _signInManager.SignInAsync(user, false);
                await _loginService.LogExternalLoginAttemptAsync(email.Value, provider, true);

                // delete temporary cookie used during external authentication
                await HttpContext.SignOutAsync(Duende.IdentityServer.IdentityServerConstants.ExternalCookieAuthenticationScheme);

                _log.LogInformation("User {Username} logged in with {Provider} provider.", user.Username, provider);

                // validate return URL and redirect back to authorization endpoint or a local page
                var returnUrl = result.Properties?.Items["returnUrl"];

                if (_interaction.IsValidReturnUrl(returnUrl) || Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl!);
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

        await _loginService.LogExternalLoginAttemptAsync(email.Value, provider ?? "NULL PROVIDER", false);

        return View();
    }

    [HttpGet("logout")]
    public async Task<IActionResult> Logout(string logoutId)
    {
        var logout = await _interaction.GetLogoutContextAsync(logoutId);

        await _signInManager.SignOutAsync();

        return Redirect(logout?.PostLogoutRedirectUri ?? "/login");
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
        ArgumentNullException.ThrowIfNull(model);

        model.WasEmailAttempted = true;

        if (ModelState.IsValid)
        {
            var user = await _userMgr.FindByEmailAsync(model.Email);

            if (user == null)
            {
                _log.LogInformation("Unable to find user with email [{Email}].", model.Email);

                return View(model);
            }

            // legacy users might not have the security stamp set.  if so, set it here, as a non-null security stamp is required for this to work
            if (string.IsNullOrEmpty(user.SecurityStamp))
            {
                await _userMgr.UpdateSecurityStampAsync(user);
            }

            var code = await _userMgr.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action("ResetPassword", "Account", new { model.Email, code }, Request.Scheme);
            var emailModel = new ResetPasswordEmailModel("Reset Password", callbackUrl!);
            var body = await _razorRenderer.RenderViewToStringAsync("~/Views/Email/ResetPassword.cshtml", emailModel);

            _log.LogInformation("Sending password reset email to user: {User}", user.Name);

            await _emailService.SendHtmlAsync(model.Email, EmailFrom, "Reset Password for mikeandwan.us", body);

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

        await TryUpdateModelAsync(model, string.Empty, x => x.Email, x => x.Code);
        ModelState.Clear();

        return View(model);
    }

    [HttpPost("reset-password")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        model.ResetAttempted = true;

        if (ModelState.IsValid)
        {
            var user = await _repo.GetUserByEmailAsync(model.Email!);

            if(user == null)
            {
                return NotFound();
            }

            var result = await _userMgr.ResetPasswordAsync(user, model.Code, model.NewPassword);

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
        ArgumentNullException.ThrowIfNull(model);

        model.ChangeAttempted = true;

        if (ModelState.IsValid)
        {
            var user = await _repo.GetUserAsync(User.GetUsername());

            if(user == null)
            {
                return NotFound();
            }

            var result = await _userMgr.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

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
        ViewBag.States = await GetStateSelectListItemsAsync();
        ViewBag.Countries = await GetCountrySelectListItemsAsync();

        var user = await _userMgr.FindByNameAsync(User.GetUsername());

        if(user == null)
        {
            return BadRequest();
        }

        var model = new ProfileModel
        {
            Username = user.Username,
            FirstName = user.FirstName ?? string.Empty,
            LastName = user.LastName ?? string.Empty,
            Email = user.Email ?? string.Empty,
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
        ArgumentNullException.ThrowIfNull(model);

        ViewBag.States = await GetStateSelectListItemsAsync();
        ViewBag.Countries = await GetCountrySelectListItemsAsync();

        model.WasAttempted = true;
        model.Username = User.GetUsername();

        if (ModelState.IsValid)
        {
            var user = await _userMgr.FindByNameAsync(model.Username);

            if(user == null)
            {
                return BadRequest();
            }

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;
            user.IsGithubAuthEnabled = model.EnableGithubAuth;
            user.IsGoogleAuthEnabled = model.EnableGoogleAuth;
            user.IsMicrosoftAuthEnabled = model.EnableMicrosoftAuth;
            user.IsTwitterAuthEnabled = model.EnableTwitterAuth;

            await _repo.UpdateUserAsync(user);

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
        var states = await _repo.GetStatesAsync();

        return states.Select(x => new SelectListItem
        {
            Value = x.Code,
            Text = x.Name
        });
    }

    async Task<IEnumerable<SelectListItem>> GetCountrySelectListItemsAsync()
    {
        var countries = await _repo.GetCountriesAsync();

        return countries.Select(x => new SelectListItem
        {
            Value = x.Code,
            Text = x.Name
        });
    }

    async Task<IEnumerable<ExternalLoginScheme>> GetExternalLoginSchemes()
    {
        var schemes = await _signInManager.GetExternalAuthenticationSchemesAsync();

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

    RedirectResult RedirectToLocal(string returnUrl)
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
