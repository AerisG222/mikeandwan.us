using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MawMvcApp.ViewModels.Navigation;
using Microsoft.AspNetCore.Authorization;
using System;
using Maw.Security;
using System.Threading.Tasks;

namespace MawMvcApp.Controllers
{
    [Route("")]
    public class HomeController
        : MawBaseController<HomeController>
    {
        readonly IAuthorizationService _authzService;


		public HomeController(ILogger<HomeController> log,
                              IAuthorizationService authorizationService)
			: base(log)
		{
            _authzService = authorizationService ?? throw new ArgumentNullException(nameof(authorizationService));
		}


        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
			ViewBag.NavigationZone = NavigationZone.Home;

            var canViewPhotos = (await _authzService.AuthorizeAsync(HttpContext.User, null, Policy.ViewPhotos)).Succeeded;

            return View(canViewPhotos);
        }
    }
}
