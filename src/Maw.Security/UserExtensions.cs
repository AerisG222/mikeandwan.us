using System.Security.Claims;

namespace Maw.Security;

public static class UserExtensions
{
    public static string[] GetAllRoles(this ClaimsPrincipal user)
    {
        ArgumentNullException.ThrowIfNull(user);

        var roles = user.Identities
            .SelectMany(identity => identity.FindAll(identity.RoleClaimType))
            .Select(claim => claim.Value)
            .ToArray();

        return roles;
    }

    public static bool IsAdmin(this ClaimsPrincipal user)
    {
        ArgumentNullException.ThrowIfNull(user);

        return user.IsInRole(Role.Admin);
    }
}
