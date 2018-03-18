using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Maw.Domain.Identity;
using Maw.Security;


namespace MawApi.Controllers
{
	[ApiController]
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
    }
}
