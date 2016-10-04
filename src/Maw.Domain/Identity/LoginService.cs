using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;


namespace Maw.Domain.Identity
{
	public class LoginService
		: ILoginService
	{
		readonly IUserRepository _repo;
		readonly SignInManager<MawUser> _signInManager;
		readonly UserManager<MawUser> _userMgr;
		readonly ILogger _log;
		
		
		public LoginService(IUserRepository repo, 
		                    SignInManager<MawUser> signInManager, 
							UserManager<MawUser> userManager, 
							ILogger<LoginService> log)
		{
			if(repo == null)
			{
				throw new ArgumentNullException(nameof(repo));
			}
			
			if(signInManager == null)
			{
				throw new ArgumentNullException(nameof(signInManager));
			}
			
			if(userManager == null)
			{
				throw new ArgumentNullException(nameof(userManager));
			}
			
			if(log == null)
			{
				throw new ArgumentNullException(nameof(log));
			}
			
			_repo = repo;
			_signInManager = signInManager;
			_userMgr = userManager;
			_log = log;
		}
		
		
		public async Task<SignInResult> AuthenticateAsync(string username, string password, short loginArea)
		{
			try
			{
	            var result = await _signInManager.PasswordSignInAsync(username, password, isPersistent: false, lockoutOnFailure: false);
				var activityType = result == SignInResult.Success ? (short)1 : (short)2;

				await _repo.AddLoginHistoryAsync(username, activityType, loginArea);

				if(result == SignInResult.Success)
	            {
					_log.LogInformation(string.Concat(username, " logged in successfully against new authentication system"));
	            }
				
				return result;
			}
			catch(Exception ex)
			{
				_log.LogWarning(string.Concat("Unable to authenticate user [", username, "].", ex));
			}
			
			return SignInResult.Failed;
		}
	}
}
