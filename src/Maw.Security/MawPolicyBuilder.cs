using Microsoft.AspNetCore.Authorization;

namespace Maw.Security;

public static class MawPolicyBuilder
{
    public static void AddMawPolicies(AuthorizationOptions opts)
    {
        ArgumentNullException.ThrowIfNull(opts);

        opts.AddPolicy(MawPolicy.ViewPhotos, new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());
        opts.AddPolicy(MawPolicy.ViewVideos, new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());
        opts.AddPolicy(MawPolicy.AdminPhotos, new AuthorizationPolicyBuilder().RequireRole(Role.Admin).Build());
        opts.AddPolicy(MawPolicy.AdminVideos, new AuthorizationPolicyBuilder().RequireRole(Role.Admin).Build());
        opts.AddPolicy(MawPolicy.AdminSite, new AuthorizationPolicyBuilder().RequireRole(Role.Admin).Build());
        opts.AddPolicy(MawPolicy.CanUpload, new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());
    }
}
