using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using MawMvcApp.ViewModels;
using MawMvcApp.ViewModels.Navigation;
using Maw.Security;

namespace MawMvcApp.ViewComponents;

public class PrimaryNav
    : ViewComponent
{
    readonly IAuthorizationService _authzService;
    readonly UrlConfig _urlConfig;

    public PrimaryNav(IAuthorizationService authorizationService, IOptions<UrlConfig> urlConfig)
    {
        ArgumentNullException.ThrowIfNull(authorizationService);
        ArgumentNullException.ThrowIfNull(urlConfig?.Value);

        _authzService = authorizationService;
        _urlConfig = urlConfig?.Value!;
    }

    public async Task<IViewComponentResult> InvokeAsync(NavigationZone activeZone)
    {
        var model = new PrimaryNavViewModel
        {
            ActiveNavigationZone = activeZone,
            AuthorizedForAdmin = (await _authzService.AuthorizeAsync(HttpContext.User, null, MawPolicy.AdminSite)).Succeeded,
            UrlConfig = _urlConfig
        };

        return View(model);
    }
}
