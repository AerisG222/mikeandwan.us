using System.Security.Claims;

namespace Maw.Domain;

public static class ClaimsPrincipalExtensions
{
    public static string GetUsername(this ClaimsPrincipal user)
    {
        ArgumentNullException.ThrowIfNull(user.Identity?.Name);

        return user.Identity.Name;
    }
}
