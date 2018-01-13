using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Maw.Domain.Email;
using Maw.Domain.Identity;
using MawMvcApp.ViewModels.About;
using MawMvcApp.ViewModels.Account;
using MawMvcApp.ViewModels.Email;
using MawMvcApp.ViewModels.Navigation;
using Mvc.RenderViewToString;


namespace MawMvcApp.Controllers
{
	[Route("account")]
    public class AccountController
        : MawBaseController<AccountController>
    {
		const byte LOGIN_AREA_FORM = 1;

        readonly IUserRepository _repo;
		readonly ContactConfig _contactConfig;
		readonly IEmailService _emailService;
		readonly RazorViewToStringRenderer _razorRenderer;


		public AccountController(ILogger<AccountController> log,
								 IOptions<ContactConfig> contactOpts,
								 IUserRepository userRepository,
								 IEmailService emailService,
								 RazorViewToStringRenderer razorRenderer)
			: base(log)
        {
			_contactConfig = contactOpts.Value;
            _repo = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
			_emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
			_razorRenderer = razorRenderer ?? throw new ArgumentNullException(nameof(razorRenderer));
        }


		[HttpGet("login")]
		public IActionResult Login(string returnUrl = null)
		{
			ViewBag.NavigationZone = NavigationZone.Account;
			ViewBag.ReturnUrl = returnUrl;

            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

			var vm = new LoginModel();

			return View(vm);
		}


		[Authorize]
		[HttpGet("access-denied")]
		public IActionResult AccessDenied()
		{
			ViewBag.NavigationZone = NavigationZone.Account;

			return View();
		}


		[Authorize]
		[HttpGet("logout")]
		public IActionResult Logout()
		{
			//await _signInManager.SignOutAsync();

			return RedirectToAction("Index", "Home");
		}


		[HttpGet("forgot-password")]
		public IActionResult ForgotPassword()
		{
			ViewBag.NavigationZone = NavigationZone.Account;

			return View(new ForgotPasswordModel());
		}

/*
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
*/

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
    }
}
