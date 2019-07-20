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
            if(authorizationService == null)
			{
				throw new ArgumentNullException(nameof(authorizationService));
			}

            _authzService = authorizationService;
        }


        public async Task<IViewComponentResult> InvokeAsync(NavigationZone activeZone)
        {
            var model = new PrimaryNavViewModel();

            model.ActiveNavigationZone = activeZone;

            model.AuthorizedForAdmin = (await _authzService.AuthorizeAsync(HttpContext.User, null, Policy.AdminSite).ConfigureAwait(false)).Succeeded;

            return View(model);
        }
    }
}
