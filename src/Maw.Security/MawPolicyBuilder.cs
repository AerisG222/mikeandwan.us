using Microsoft.AspNetCore.Authorization;


namespace Maw.Security
{
    public static class MawPolicyBuilder
    {
        public static void AddMawPolicies(AuthorizationOptions opts)
        {
            opts.AddPolicy(Policy.ViewPhotos, new AuthorizationPolicyBuilder().RequireRole(Role.Friend, Role.Admin).Build());
            opts.AddPolicy(Policy.ViewVideos, new AuthorizationPolicyBuilder().RequireRole(Role.Friend, Role.Admin).Build());
            opts.AddPolicy(Policy.AdminSite, new AuthorizationPolicyBuilder().RequireRole(Role.Admin).Build());
            opts.AddPolicy(Policy.CanUpload, new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());
        }
    }
}
