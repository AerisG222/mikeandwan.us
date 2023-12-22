using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Maw.Domain.Models.Identity;

namespace MawAuth.Services;

public class IdentityServerProfileService
        : IProfileService
{
    readonly UserManager<MawUser> _usrMgr;
    readonly ILogger _log;

    public IdentityServerProfileService(
        ILogger<IdentityServerProfileService> log,
        UserManager<MawUser> userManager)
    {
        ArgumentNullException.ThrowIfNull(log);
        ArgumentNullException.ThrowIfNull(userManager);

        _log = log;
        _usrMgr = userManager;
    }

    public async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        _log.LogDebug("requested claims:");

        foreach (var c in context.RequestedClaimTypes)
        {
            _log.LogDebug("{RequestedClaimType}", c);
        }

        _log.LogDebug("src subject claims:");
        PrintClaims(context.Subject.Claims);

        var u = await _usrMgr.GetUserAsync(context.Subject);

        _log.LogDebug("user subject claims:");
        PrintClaims(u?.Claims);

        context.LogProfileRequest(_log);
        context.AddRequestedClaims(u?.Claims);
        context.LogIssuedClaims(_log);

        _log.LogDebug("issued claims:");
        PrintClaims(context.IssuedClaims);
    }

    public virtual Task IsActiveAsync(IsActiveContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        _log.LogDebug("IsActive called from: {Caller}", context.Caller);

        context.IsActive = true;
        return Task.CompletedTask;
    }

    void PrintClaims(IEnumerable<Claim>? claims)
    {
        if(claims == null)
        {
            _log.LogDebug("Claims are null");

            return;
        }

        foreach (var c in claims)
        {
            _log.LogDebug("{ClaimType}: {ClaimValue}", c.Type, c.Value);
        }
    }
}
