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
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;


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


		[Authorize]
		[HttpGet("login")]
		public IActionResult Login()
		{
			// q: why is the login method marked with the authorize attribute?
			// a: because i am lazy.  this will trigger the oidc authentication flow
			//    without any additional code.  if we get in this method then the user
			//    is logged in, and we can just go to the homepage
			return LocalRedirect("/");
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
			return new SignOutResult(new[] {
				CookieAuthenticationDefaults.AuthenticationScheme,
				OpenIdConnectDefaults.AuthenticationScheme
			});
		}


		/*
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
