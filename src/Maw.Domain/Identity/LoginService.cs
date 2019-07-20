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
			_repo = repo ?? throw new ArgumentNullException(nameof(repo));
			_signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
			_userMgr = userManager ?? throw new ArgumentNullException(nameof(userManager));
			_log = log ?? throw new ArgumentNullException(nameof(log));
		}


		public async Task<SignInResult> AuthenticateAsync(string username, string password, short loginArea)
		{
			try
			{
	            var result = await _signInManager.PasswordSignInAsync(username, password, isPersistent: false, lockoutOnFailure: false).ConfigureAwait(false);
				var activityType = result == SignInResult.Success ? (short)1 : (short)2;

				await _repo.AddLoginHistoryAsync(username, activityType, loginArea).ConfigureAwait(false);

				if(result == SignInResult.Success)
	            {
					_log.LogInformation(string.Concat(username, " logged in successfully against new authentication system"));
	            }

				return result;
			}
			catch(Exception ex)
			{
				_log.LogWarning(ex, string.Concat("Unable to authenticate user [", username, "]."));
			}

			return SignInResult.Failed;
		}


		public Task LogExternalLoginAttemptAsync(string email, string provider, bool wasSuccessful)
		{
            if(provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

			var activityType = wasSuccessful ? (short) 1: (short) 2;
			var area = (short)0;

			switch(provider.ToUpperInvariant())
			{
				case "GITHUB":
					area = 3;
					break;
				case "GOOGLE":
					area = 4;
					break;
				case "MICROSOFT":
					area = 5;
					break;
				case "TWITTER":
					area = 6;
					break;
				default:
					throw new Exception("Invalid login area specified!");
			}

			return _repo.AddExternalLoginHistoryAsync(email, activityType, area);
		}
	}
}
