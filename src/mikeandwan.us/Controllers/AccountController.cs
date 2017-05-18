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
using System.Security.Claims;
using Microsoft.AspNetCore.Http;


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
		//readonly string _externalCookieScheme;


		public AccountController(ILogger<AccountController> log, 
								 IOptions<ContactConfig> contactOpts, 
								 IUserRepository userRepository, 
			                     SignInManager<MawUser> signInManager, 
								 UserManager<MawUser> userManager, 
								 IEmailService emailService,
								 ILoginService loginService,
								 IOptions<IdentityCookieOptions> identityCookieOptions)
			: base(log)
        {
			_contactConfig = contactOpts.Value;
            _repo = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
			_userMgr = userManager ?? throw new ArgumentNullException(nameof(userManager));
			_emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
			_loginService = loginService ?? throw new ArgumentNullException(nameof(loginService));

			// TODO: can we kill this?
			//_externalCookieScheme = identityCookieOptions.Value.ExternalCookieAuthenticationScheme;
        }

		
		[HttpGet("login")]
		public async Task<ActionResult> Login(string returnUrl = null)
		{
			ViewBag.NavigationZone = NavigationZone.Account;
			ViewBag.ReturnUrl = returnUrl;

			// TODO: kill?
			// Clear the existing external cookie to ensure a clean login process
            //await HttpContext.Authentication.SignOutAsync(_externalCookieScheme);

            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

			var vm = new LoginModel
			{
				ExternalSchemes = await GetExternalLoginSchemes()
			};

			return View(vm);
		}

		
		[HttpPost("login")]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Login(LoginModel model, string returnUrl = null)
        {
			ViewBag.NavigationZone = NavigationZone.Account;
			ViewBag.ReturnUrl = returnUrl;
			
			model.ExternalSchemes = await GetExternalLoginSchemes();
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
		

		[HttpGet("external-login")]
		public async Task<ActionResult> ExternalLogin(string provider, string returnUrl = null)
		{
			var schemes = await _signInManager.GetExternalAuthenticationSchemesAsync();

			if(!schemes.Any(x => string.Equals(x.Name, provider, StringComparison.OrdinalIgnoreCase)))
			{
				_log.LogError($"Invalid external authentication scheme specified: {provider}");
				return Redirect(nameof(Login));
			}

			var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { ReturnUrl = returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

        	return Challenge(properties, provider);
		} 


		[HttpGet("external-login-callback")]
		public async Task<ActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
		{
			ViewBag.NavigationZone = NavigationZone.Account;
			
			if(!string.IsNullOrEmpty(remoteError))
			{
				_log.LogError($"Unable to authenticate externally: {remoteError}");
				return View();
			}

			var extLoginInfo = await _signInManager.GetExternalLoginInfoAsync();

			if(extLoginInfo == null)
			{
				_log.LogError("Unable to obtain external login info");
				return View();
			}

			var schemes = await _signInManager.GetExternalAuthenticationSchemesAsync();

			if(!schemes.Any(x => string.Equals(x.Name, extLoginInfo.LoginProvider, StringComparison.OrdinalIgnoreCase)))
			{
				_log.LogError($"External provider {extLoginInfo.LoginProvider} is not supported");
				return View();
			}

			var email = extLoginInfo.Principal?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email);

			if(email == null)
			{
				_log.LogError($"Unable to obtain email from External Authentication Provider {extLoginInfo.LoginProvider}");
				return View();
			}

			var user = await _userMgr.FindByEmailAsync(email.Value);

            if (user != null)
            {
				if(user.IsExternalAuthEnabled(extLoginInfo.LoginProvider))
				{
					await _signInManager.SignInAsync(user, false);
					await _loginService.LogExternalLoginAttemptAsync(email.Value, extLoginInfo.LoginProvider, true);
					
					_log.LogInformation($"User {user.Username} logged in with {extLoginInfo.LoginProvider} provider.");

					if(!string.IsNullOrEmpty(returnUrl))
					{
						return Redirect(returnUrl);
					}

					return Redirect("/");
				}
				else
				{
					_log.LogError($"User {user.Username} unable to login with {extLoginInfo.LoginProvider} as they have not yet opted-in for this provider.");
				}
            }

			await _loginService.LogExternalLoginAttemptAsync(email.Value, extLoginInfo.LoginProvider, false);

			return View();
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
    }
}
