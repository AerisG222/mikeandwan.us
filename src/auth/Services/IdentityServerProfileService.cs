using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Maw.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;


namespace MawAuth.Services
{
    public class IdentityServerProfileService
		  : IProfileService
    {
        readonly UserManager<MawUser> _usrMgr;
        readonly ILogger _log;


        public IdentityServerProfileService(ILogger<IdentityServerProfileService> log, UserManager<MawUser> userManager)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _usrMgr = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }


        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            _log.LogDebug("requested claims:");

            foreach(var c in context.RequestedClaimTypes)
            {
                _log.LogDebug(c);
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
            _log.LogDebug("IsActive called from: {caller}", context.Caller);

            context.IsActive = true;
            return Task.CompletedTask;
        }


        void PrintClaims(IEnumerable<Claim> claims)
        {
            foreach(var c in claims)
			{
				_log.LogInformation($"{c.Type}: {c.Value}");
			}
        }
    }
}
