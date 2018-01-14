using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Maw.Domain.Identity;
//using MawMvcApp.ViewModels.Account;
using SignInRes = Microsoft.AspNetCore.Identity.SignInResult;
using Maw.Security;
using Maw.Security.Filters;


namespace MawMvcApp.Controllers
{
	[Route("account")]
    public class AccountController
        : ControllerBase
    {
		const byte LOGIN_AREA_API = 2;

		readonly ILoginService _loginService;


		public AccountController(ILoginService loginService)
        {
			_loginService = loginService ?? throw new ArgumentNullException(nameof(loginService));
        }

		/*
        [HttpPost("login")]
        public async Task<bool> Login(LoginModel model)
        {
			var result = await _loginService.AuthenticateAsync(model.Username, model.Password, LOGIN_AREA_API);

			return result == SignInRes.Success;
        }
		*/

		[HttpGet("get-xsrf-token")]
		[TypeFilter(typeof(ApiAntiforgeryActionFilter))]
		public IActionResult GetXsrfToken() {
			return Ok();
		}
    }
}
