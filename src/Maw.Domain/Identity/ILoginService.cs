using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;


namespace Maw.Domain.Identity
{
	public interface ILoginService
    {
        Task<SignInResult> AuthenticateAsync(string username, string password, short loginArea);
        Task LogExternalLoginAttemptAsync(string email, string provider, bool wasSuccessful);
    }
}
