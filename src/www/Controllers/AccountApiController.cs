using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Maw.Domain.Identity;
using MawMvcApp.Filters;
using MawMvcApp.ViewModels.Account;
using SignInRes = Microsoft.AspNetCore.Identity.SignInResult;


namespace MawMvcApp.Controllers
{
	[Route("api/account")]
    public class AccountApiController
        : MawBaseController<AccountApiController>
    {
		const byte LOGIN_AREA_API = 2;

		readonly ILoginService _loginService;


		public AccountApiController(ILogger<AccountApiController> log,
									ILoginService loginService)
			: base(log)
        {
			_loginService = loginService ?? throw new ArgumentNullException(nameof(loginService));
        }


        [HttpPost("login")]
        public async Task<bool> Login(LoginModel model)
        {
			var result = await _loginService.AuthenticateAsync(model.Username, model.Password, LOGIN_AREA_API);

			return result == SignInRes.Success;
        }


		[HttpGet("get-xsrf-token")]
		[TypeFilter(typeof(ApiAntiforgeryActionFilter))]
		public IActionResult GetXsrfToken() {
			return Ok();
		}
    }
}
