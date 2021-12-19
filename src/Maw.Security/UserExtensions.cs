using System;
using System.Linq;
using System.Security.Claims;

namespace Maw.Security
{
    public static class UserExtensions
    {
        public static string[] GetAllRoles(this ClaimsPrincipal user)
        {
            if(user == null) {
                throw new ArgumentNullException(nameof(user));
            }

            var roles = user.Identities
                .SelectMany(identity => identity.FindAll(identity.RoleClaimType))
                .Select(claim => claim.Value)
                .ToArray();

            return roles;
        }
    }

}
