using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Maw.Domain.Identity;
using MawMvcApp.ViewModels.Account;
using SignInRes = Microsoft.AspNetCore.Identity.SignInResult;


namespace MawMvcApp.Controllers
{
	[Route("api/account")]
    public class AccountApiController 
        : MawBaseController
    {
		const byte LOGIN_AREA_API = 2;
		
        readonly IUserRepository _repo;
		readonly SignInManager<MawUser> _signInManager;
		readonly UserManager<MawUser> _userMgr;


		public AccountApiController(IAuthorizationService authorizationService, ILoggerFactory loggerFactory, IUserRepository userRepository, SignInManager<MawUser> signInManager,
			                        UserManager<MawUser> userManager)
			: base(authorizationService, loggerFactory)
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

            _repo = userRepository;
            _signInManager = signInManager;
			_userMgr = userManager;
        }


        [HttpPost("login")]
        public async Task<bool> Login(LoginModel model)
        {
			var ls = new LoginService(_repo, _signInManager, _userMgr, _loggerFactory);
			var result = await ls.AuthenticateAsync(model.Username, model.Password, LOGIN_AREA_API);
						
			return result == SignInRes.Success;
        }
    }
}
