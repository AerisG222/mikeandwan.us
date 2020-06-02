using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MawMvcApp.ViewModels.Navigation;
using Maw.Security;


namespace MawMvcApp.ViewComponents
{
    public class PrimaryNav
        : ViewComponent
    {
        readonly IAuthorizationService _authzService;


        public PrimaryNav(IAuthorizationService authorizationService)
        {
            _authzService = authorizationService ?? throw new ArgumentNullException(nameof(authorizationService));
        }


        public async Task<IViewComponentResult> InvokeAsync(NavigationZone activeZone)
        {
            var model = new PrimaryNavViewModel {
                ActiveNavigationZone = activeZone,
                AuthorizedForAdmin = (await _authzService.AuthorizeAsync(HttpContext.User, null, MawPolicy.AdminSite).ConfigureAwait(false)).Succeeded
            };

            return View(model);
        }
    }
}
