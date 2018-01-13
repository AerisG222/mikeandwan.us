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
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Maw.Domain.Email;
using Maw.Domain.Identity;
//using MawMvcApp.ViewModels.About;
using MawMvcApp.ViewModels.Account;
//using MawMvcApp.ViewModels.Email;
//using MawMvcApp.ViewModels.Navigation;
using SignInRes = Microsoft.AspNetCore.Identity.SignInResult;
using Microsoft.AspNetCore.Authentication;
using IdentityServer4.Services;
//using Mvc.RenderViewToString;


namespace MawMvcApp.Controllers
{
	[Route("account")]
    public class AccountController
		: Controller
//        : MawBaseController<AccountController>
    {
		const byte LOGIN_AREA_FORM = 1;
		const string LoginProviderKey = "LoginProvider";

		readonly ILogger<AccountController> _log;
		readonly IIdentityServerInteractionService _interaction;
        readonly IUserRepository _repo;
//		readonly ContactConfig _contactConfig;
		readonly SignInManager<MawUser> _signInManager;
		readonly UserManager<MawUser> _userMgr;
		//readonly IEmailService _emailService;
		readonly ILoginService _loginService;
//		readonly RazorViewToStringRenderer _razorRenderer;


		public AccountController(ILogger<AccountController> log,
								 IIdentityServerInteractionService interaction,
//								 IOptions<ContactConfig> contactOpts,
								 IUserRepository userRepository,
			                     SignInManager<MawUser> signInManager,
								 UserManager<MawUser> userManager,
//								 IEmailService emailService,
								 ILoginService loginService)
//								 RazorViewToStringRenderer razorRenderer)
//			: base(log)
        {
			_log = log ?? throw new ArgumentNullException(nameof(log));
			_interaction = interaction ?? throw new ArgumentNullException(nameof(interaction));
//			_contactConfig = contactOpts.Value;
            _repo = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
			_userMgr = userManager ?? throw new ArgumentNullException(nameof(userManager));
//			_emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
			_loginService = loginService ?? throw new ArgumentNullException(nameof(loginService));
//			_razorRenderer = razorRenderer ?? throw new ArgumentNullException(nameof(razorRenderer));
        }


		[HttpGet("login")]
		public async Task<IActionResult> Login(string returnUrl)
		{
			//ViewBag.NavigationZone = NavigationZone.Account;
			//ViewBag.ReturnUrl = returnUrl;

            //if (User.Identity.IsAuthenticated)
            //{
            //    return RedirectToAction("Index", "Home");
            //}

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
			//ViewBag.NavigationZone = NavigationZone.Account;
			//ViewBag.ReturnUrl = returnUrl;

			//model.ExternalSchemes = await GetExternalLoginSchemes();
			model.WasAttempted = true;

            if(!ModelState.IsValid)
            {
				LogValidationErrors();

                return View(model);
            }

			var result = await _loginService.AuthenticateAsync(model.Username, model.Password, LOGIN_AREA_FORM);

			_log.LogInformation("Login complete");

			if(result == SignInRes.Success)
			{
				return RedirectToLocal(model.ReturnUrl);

				/*
				if(string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
				{
                    return RedirectToAction("Index", "Home");
				}

				return Redirect(returnUrl);
				*/
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
			//var schemes = await _signInManager.GetExternalAuthenticationSchemesAsync();

			//if(!schemes.Any(x => string.Equals(x.Name, provider, StringComparison.OrdinalIgnoreCase)))
			//{
			//	_log.LogError($"Invalid external authentication scheme specified: {provider}");
			//	return RedirectToAction(nameof(Login));
			//}

			//var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { ReturnUrl = returnUrl });
            //var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

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
			//ViewBag.NavigationZone = NavigationZone.Account;

			var result = await HttpContext.AuthenticateAsync(IdentityServer4.IdentityServerConstants.ExternalCookieAuthenticationScheme);
			var items = result?.Properties?.Items;

			//var extLoginInfo = await _signInManager.GetExternalLoginInfoAsync();

			if(result?.Succeeded != true || items == null || !items.ContainsKey("scheme"))
			{
				_log.LogError("Unable to obtain external login info");
				return View();
			}

			var provider = items["scheme"];

			/*
			var schemes = await _signInManager.GetExternalAuthenticationSchemesAsync();

			if(!schemes.Any(x => string.Equals(x.Name, extLoginInfo.LoginProvider, StringComparison.OrdinalIgnoreCase)))
			{
				_log.LogError($"External provider {extLoginInfo.LoginProvider} is not supported");
				return View();
			}
			*/

			var email = result.Principal?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email);

			if(email == null)
			{
				_log.LogError($"Unable to obtain email from External Authentication Provider {provider}");
				return View();
			}

			var user = await _userMgr.FindByEmailAsync(email.Value);

            if (user != null)
            {
				if(user.IsExternalAuthEnabled(provider))
				{
					// now sign in the local user
					await _signInManager.SignInAsync(user, false);
					await _loginService.LogExternalLoginAttemptAsync(email.Value, provider, true);

					// delete temporary cookie used during external authentication
					await HttpContext.SignOutAsync(IdentityServer4.IdentityServerConstants.ExternalCookieAuthenticationScheme);

					_log.LogInformation($"User {user.Username} logged in with {provider} provider.");

					// validate return URL and redirect back to authorization endpoint or a local page
					var returnUrl = result.Properties.Items["returnUrl"];

					if (_interaction.IsValidReturnUrl(returnUrl) || Url.IsLocalUrl(returnUrl))
					{
						return Redirect(returnUrl);
					}
				}
				else
				{
					_log.LogError($"User {user.Username} unable to login with {provider} as they have not yet opted-in for this provider.");
				}
            }

			await _loginService.LogExternalLoginAttemptAsync(email.Value, provider, false);

			return View();
		}

/*

		[Authorize]
		[HttpGet("access-denied")]
		public IActionResult AccessDenied()
		{
			ViewBag.NavigationZone = NavigationZone.Account;

			return View();
		}


		[Authorize]
		[HttpGet("logout")]
		public async Task<IActionResult> Logout()
		{
			await _signInManager.SignOutAsync();

			return RedirectToAction("Index", "Home");
		}


		[HttpGet("forgot-password")]
		public IActionResult ForgotPassword()
		{
			ViewBag.NavigationZone = NavigationZone.Account;

			return View(new ForgotPasswordModel());
		}


		[HttpPost("forgot-password")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
		{
			ViewBag.NavigationZone = NavigationZone.Account;

			model.WasEmailAttempted = true;

			if(ModelState.IsValid)
			{
				var user = await _userMgr.FindByEmailAsync(model.Email);

				// legacy users might not have the security stamp set.  if so, set it here, as a non-null security stamp is requilred for this to work
				if(string.IsNullOrEmpty(user.SecurityStamp))
				{
					await _userMgr.UpdateSecurityStampAsync(user);
				}

				var code = await _userMgr.GeneratePasswordResetTokenAsync(user);
				var callbackUrl = Url.Action("ResetPassword", "Account", new { user.Email, code }, Request.Scheme);

                _log.LogInformation($"user: {user.Name}");
                _log.LogInformation($"code: {code}");
                _log.LogInformation($"reset url: {callbackUrl}");

				var emailModel = new ResetPasswordEmailModel
					{
						Title = "Reset Password",
						CallbackUrl = callbackUrl
					};

				var body = await _razorRenderer.RenderViewToStringAsync("~/Views/Email/ResetPassword.cshtml", emailModel).ConfigureAwait(false);

				await _emailService.SendHtmlAsync(user.Email, _contactConfig.To, "Reset Password for mikeandwan.us", body).ConfigureAwait(false);

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
			ViewBag.NavigationZone = NavigationZone.Account;

			var model = new ResetPasswordModel();

			await TryUpdateModelAsync<ResetPasswordModel>(model, string.Empty, x =>  x.Email, x => x.Code);
			ModelState.Clear();

			return View(model);
		}


		[HttpPost("reset-password")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
		{
			ViewBag.NavigationZone = NavigationZone.Account;
			model.ResetAttempted = true;

			if(ModelState.IsValid)
			{
				var user = await _repo.GetUserByEmailAsync(model.Email);
				var result = await _userMgr.ResetPasswordAsync(user, model.Code, model.NewPassword);

				if(result.Succeeded)
				{
					model.WasReset = true;
				}
				else
				{
					_log.LogWarning(result.ToString());

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
		[HttpGet("edit-profile")]
		public async Task<IActionResult> EditProfile()
		{
			ViewBag.NavigationZone = NavigationZone.Account;

			ViewBag.States = await GetStateSelectListItemsAsync();
			ViewBag.Countries = await GetCountrySelectListItemsAsync();

			var user = await _userMgr.FindByNameAsync(User.Identity.Name);

			var model = new ProfileModel
			{
				Username = user.Username,
				FirstName = user.FirstName,
				LastName = user.LastName,
				DateOfBirth = user.DateOfBirth,
				Company = user.Company,
				Position = user.Position,
				PersonalEmail = user.Email,
				WorkEmail = user.WorkEmail,
				HomePhone = user.HomePhone,
				MobilePhone = user.MobilePhone,
				WorkPhone = user.WorkPhone,
				Address1 = user.Address1,
				Address2 = user.Address2,
				City = user.City,
				State = user.State,
				PostalCode = user.PostalCode,
				Country = user.Country,
				Website = user.Website,
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
			ViewBag.NavigationZone = NavigationZone.Account;
			ViewBag.States = await GetStateSelectListItemsAsync();
			ViewBag.Countries = await GetCountrySelectListItemsAsync();

			model.WasAttempted = true;
            model.Username = User.Identity.Name;

			if(ModelState.IsValid)
			{
				var user = await _userMgr.FindByNameAsync(User.Identity.Name);

				user.FirstName = model.FirstName;
				user.LastName = model.LastName;
				user.DateOfBirth = model.DateOfBirth;
				user.Company = model.Company;
				user.Position = model.Position;
				user.Email = model.PersonalEmail;
				user.WorkEmail = model.WorkEmail;
				user.HomePhone = model.HomePhone;
				user.MobilePhone = model.MobilePhone;
				user.WorkPhone = model.WorkPhone;
				user.Address1 = model.Address1;
				user.Address2 = model.Address2;
				user.City = model.City;
				user.State = model.State;
				user.PostalCode = model.PostalCode;
				user.Country = model.Country;
				user.Website = model.Website;
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


		[Authorize]
		[HttpGet("change-password")]
		public IActionResult ChangePassword()
		{
			ViewBag.NavigationZone = NavigationZone.Account;

			var m = new ChangePasswordModel();

			return View(m);
		}


		[HttpPost("change-password")]
		[Authorize]
        [ValidateAntiForgeryToken]
		public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
		{
			ViewBag.NavigationZone = NavigationZone.Account;
			model.ChangeAttempted = true;

			if(ModelState.IsValid)
			{
				var user = await _repo.GetUserAsync(User.Identity.Name);
				var result = await _userMgr.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

				if (result.Succeeded)
				{
					model.ChangeSucceeded = true;
				}
                else
                {
					_log.LogWarning(result.ToString());

					AddErrors(result);
                }
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
		*/

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
				_log.LogWarning(err.ErrorMessage);
			}
		}
    }
}
