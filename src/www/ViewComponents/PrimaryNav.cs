using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MawMvcApp.ViewModels;
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

            model.AuthorizedForPhotos = (await _authzService.AuthorizeAsync(HttpContext.User, null, Policy.ViewPhotos)).Succeeded;
            model.AuthorizedForVideos = (await _authzService.AuthorizeAsync(HttpContext.User, null, Policy.ViewVideos)).Succeeded;
            model.AuthorizedForAdmin = (await _authzService.AuthorizeAsync(HttpContext.User, null, Policy.AdminSite)).Succeeded;
            model.AuthorizedForUpload = (await _authzService.AuthorizeAsync(HttpContext.User, null, Policy.CanUpload)).Succeeded;

            return View(model);
        }
    }
}
