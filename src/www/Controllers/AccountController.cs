using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MawMvcApp.ViewModels.Navigation;


namespace MawMvcApp.Controllers
{
	[Route("account")]
    public class AccountController
        : MawBaseController<AccountController>
    {
		public AccountController(ILogger<AccountController> log)
			: base(log)
        {

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


		[Authorize]
		[HttpGet("spa-silent-signin")]
		public IActionResult SpaSilentSignin()
		{
			return View();
		}
    }
}
