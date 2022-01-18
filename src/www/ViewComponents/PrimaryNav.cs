using System;
using System.Threading.Tasks;
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
        _authzService = authorizationService ?? throw new ArgumentNullException(nameof(authorizationService));
        _urlConfig = urlConfig?.Value ?? throw new ArgumentNullException(nameof(urlConfig));
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
