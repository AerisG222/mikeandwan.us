using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Maw.Domain.Email;
using Maw.Domain.Identity;
using MawMvcApp.ViewModels.Account;
using MawMvcApp.ViewModels.Navigation;
using MawMvcApp.ViewModels.About;
using SignInRes = Microsoft.AspNetCore.Identity.SignInResult;


namespace MawMvcApp.Controllers
{
	[Route("account")]
    public class AccountController 
        : MawBaseController<AccountController>
    {
		const byte LOGIN_AREA_FORM = 1;
		
        readonly IUserRepository _repo;
		readonly ContactConfig _contactConfig;
		readonly SignInManager<MawUser> _signInManager;
		readonly UserManager<MawUser> _userMgr;
		readonly IEmailService _emailService;
		readonly ILoginService _loginService;


		public AccountController(IAuthorizationService authorizationService, 
		                         ILogger<AccountController> log, 
								 IOptions<ContactConfig> contactOpts, 
								 IUserRepository userRepository, 
			                     SignInManager<MawUser> signInManager, 
								 UserManager<MawUser> userManager, 
								 IEmailService emailService,
								 ILoginService loginService)
			: base(authorizationService, log)
        {
			if(userRepository == null)
			{
				throw new ArgumentNullException(nameof(userRepository));
			}

			if(signInManager == null)
			{
				throw new ArgumentNullException(nameof(signInManager));
			}

			if(userManager == null)
			{
				throw new ArgumentNullException(nameof(userManager));
			}

			if(emailService == null)
			{
				throw new ArgumentNullException(nameof(emailService));
			}
			
			if(contactOpts == null)
			{
				throw new ArgumentNullException(nameof(contactOpts));
			}

			if(loginService == null)
			{
				throw new ArgumentNullException(nameof(loginService));
			}

			_contactConfig = contactOpts.Value;
            _repo = userRepository;
            _signInManager = signInManager;
			_userMgr = userManager;
			_emailService = emailService;
			_loginService = loginService;
        }

		
		[HttpGet("login")]
		public ActionResult Login()
		{
			ViewBag.NavigationZone = NavigationZone.Account;

            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

			return View(new LoginModel());
		}

		
		[HttpPost("login")]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Login(LoginModel model, string returnUrl)
        {
			ViewBag.NavigationZone = NavigationZone.Account;

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
				if(string.IsNullOrEmpty(returnUrl))
				{
                    return RedirectToAction("Index", "Home");
				}
				else
				{
					return Redirect(returnUrl);
				}
			}
			else
			{
				ModelState.AddModelError("Error", "Sorry, a user was not found with this username/password combination");
			}
			
            return View(model);
        }
		
		
		[Authorize]
		[HttpGet("access-denied")]
		public ActionResult AccessDenied()
		{
			ViewBag.NavigationZone = NavigationZone.Account;
			
			return View();
		}
		
		
		[Authorize]
		[HttpGet("logout")]
		public async Task<ActionResult> Logout()
		{
			await _signInManager.SignOutAsync();
			
			return RedirectToAction("Index", "Home");
		}
		

		[HttpGet("forgot-password")]
		public ActionResult ForgotPassword()
		{
			ViewBag.NavigationZone = NavigationZone.Account;

			return View(new ForgotPasswordModel());
		}


		[HttpPost("forgot-password")]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ForgotPassword(ForgotPasswordModel model)
		{
			ViewBag.NavigationZone = NavigationZone.Account;

			if(ModelState.IsValid)
			{
				var user = await _userMgr.FindByEmailAsync(model.Email);
				var code = await _userMgr.GeneratePasswordResetTokenAsync(user);
				var callbackUrl = Url.Action("ResetPassword", "Account", new { user.Email, code }, Request.Scheme);

                _log.LogInformation($"user: {user.Name}");
                _log.LogInformation($"code: {code}");
                _log.LogInformation($"reset url: {callbackUrl}");
                
				await _emailService.SendHtmlAsync(user.Email, _contactConfig.To, "Reset Password for mikeandwan.us", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>.");

				model.WasEmailAttempted = true;

				return View(model);
			}
			else
			{
				LogValidationErrors();
			}

			return View(model);
		}


		[HttpGet("reset-password")]
		public async Task<ActionResult> ResetPassword(string code)
		{
			ViewBag.NavigationZone = NavigationZone.Account;

			var model = new ResetPasswordModel();

			await TryUpdateModelAsync<ResetPasswordModel>(model, string.Empty, x =>  x.Email, x => x.Code);
			ModelState.Clear();

			return View(model);
		}


		[HttpPost("reset-password")]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ResetPassword(ResetPasswordModel model)
		{
			ViewBag.NavigationZone = NavigationZone.Account;

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
					ModelState.AddModelError(nameof(model.NewPassword), "Password does not meet complexity requirements.");
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
		public async Task<ActionResult> EditProfile()
		{
			ViewBag.NavigationZone = NavigationZone.Account;
		    
			ViewBag.States = await GetStateSelectListItemsAsync();
			ViewBag.Countries = await GetCountrySelectListItemsAsync();

			var user = await _userMgr.FindByNameAsync(User.Identity.Name);

			var model = new ProfileModel {
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
				Website = user.Website
			};

			return View(model);
		}

		
		[HttpPost("edit-profile")]
		[Authorize]
        [ValidateAntiForgeryToken]
		public async Task<ActionResult> EditProfile(ProfileModel model)
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
		public ActionResult ChangePassword()
		{
			ViewBag.NavigationZone = NavigationZone.Account;
			
			var m = new ChangePasswordModel();
			
			return View(m);
		}
		
		
		[HttpPost("change-password")]
		[Authorize]
        [ValidateAntiForgeryToken]
		public async Task<ActionResult> ChangePassword(ChangePasswordModel model)
		{
			ViewBag.NavigationZone = NavigationZone.Account;
			
			if(ModelState.IsValid)
			{
				model.ChangeAttempted = true;

				var user = await _repo.GetUserAsync(User.Identity.Name);
				var result = await _userMgr.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

				if (result.Succeeded)
				{
					model.ChangeSucceeded = true;
				}
                else
                {
					_log.LogWarning(result.ToString());
					
					ModelState.AddModelError(string.Empty, "Sorry, there was an error changing your password!");
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
				
			return states.Select(x => new SelectListItem() {
                Value = x.Code, 
                Text = x.Name
            });
		}
		

		async Task<IEnumerable<SelectListItem>> GetCountrySelectListItemsAsync()
		{
			var countries = await _repo.GetCountriesAsync();
				
			return countries.Select(x => new SelectListItem() {
                Value = x.Code, 
                Text = x.Name
            });
		}
    }
}
