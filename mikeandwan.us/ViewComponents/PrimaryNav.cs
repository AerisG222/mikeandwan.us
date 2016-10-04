using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MawMvcApp.ViewModels;
using MawMvcApp.ViewModels.Navigation;


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
            model.AuthorizedForPhotos = await _authzService.AuthorizeAsync(HttpContext.User, null, MawConstants.POLICY_VIEW_PHOTOS);
            model.AuthorizedForVideos = await _authzService.AuthorizeAsync(HttpContext.User, null, MawConstants.POLICY_VIEW_VIDEOS);
            model.AuthorizedForAdmin = await _authzService.AuthorizeAsync(HttpContext.User, null, MawConstants.POLICY_ADMIN_SITE);

            return View(model);
        }
    }
}
