using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Maw.Domain.Identity;
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


		public AccountApiController(IAuthorizationService authorizationService, 
		                            ILogger<AccountApiController> log, 
									ILoginService loginService)
			: base(authorizationService, log)
        {
			if(loginService == null)
			{
				throw new ArgumentNullException(nameof(loginService));
			}

			_loginService = loginService;
        }


        [HttpPost("login")]
        public async Task<bool> Login(LoginModel model)
        {
			var result = await _loginService.AuthenticateAsync(model.Username, model.Password, LOGIN_AREA_API);
						
			return result == SignInRes.Success;
        }
    }
}
