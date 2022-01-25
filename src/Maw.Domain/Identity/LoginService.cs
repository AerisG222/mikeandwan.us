using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Maw.Data.Abstractions;
using Maw.Domain.Models.Identity;

namespace Maw.Domain.Identity;

public class LoginService
    : ILoginService
{
    readonly IUserRepository _repo;
    readonly SignInManager<MawUser> _signInManager;
    readonly ILogger _log;

    public LoginService(
        IUserRepository repo,
        SignInManager<MawUser> signInManager,
        ILogger<LoginService> log)
    {
        _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
        _log = log ?? throw new ArgumentNullException(nameof(log));
    }

    public async Task<SignInResult> AuthenticateAsync(string username, string password, short loginArea)
    {
        try
        {
            var result = await _signInManager.PasswordSignInAsync(username, password, isPersistent: false, lockoutOnFailure: false);
            var activityType = result == SignInResult.Success ? (short)1 : (short)2;

            await _repo.AddLoginHistoryAsync(username, activityType, loginArea);

            if (result == SignInResult.Success)
            {
                _log.LogInformation("{Username} logged in successfully against new authentication system", username);
            }

            return result;
        }
        catch (Exception ex)
        {
            _log.LogWarning(ex, "Unable to authenticate user {Username}.", username);
        }

        return SignInResult.Failed;
    }

    public Task LogExternalLoginAttemptAsync(string email, string provider, bool wasSuccessful)
    {
        if (provider == null)
        {
            throw new ArgumentNullException(nameof(provider));
        }

        var activityType = wasSuccessful ? (short)1 : (short)2;
        short area;

        area = provider.ToUpperInvariant() switch
        {
            "GITHUB" => 3,
            "GOOGLE" => 4,
            "MICROSOFT" => 5,
            "TWITTER" => 6,
            _ => throw new Exception("Invalid login area specified!")
        };

        return _repo.AddExternalLoginHistoryAsync(email, activityType, area);
    }
}
