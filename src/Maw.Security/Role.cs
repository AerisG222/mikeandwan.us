using System;
using System.Security.Claims;

namespace Maw.Security
{
    public static class Role
    {
        public const string Admin = "admin";
        public const string Friend = "friend";


        public static bool IsAdmin(ClaimsPrincipal user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return user.IsInRole(Role.Admin);
        }


        public static bool IsFriend(ClaimsPrincipal user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return user.IsInRole(Role.Friend);
        }
    }
}
