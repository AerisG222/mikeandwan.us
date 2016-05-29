using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MawMvcApp.ViewModels.Navigation;


namespace MawMvcApp.Controllers
{
	[Route("games")]
    public class GamesController
		: MawBaseController
    {
		public GamesController(IAuthorizationService authorizationService, ILoggerFactory loggerFactory)
			: base(authorizationService, loggerFactory)
		{

		}


		[HttpGet("")]
        public ActionResult Index()
        {
			ViewBag.NavigationZone = NavigationZone.Games;
            return View ();
        }


		[HttpGet("memory/{*extra}")]
		public ActionResult Memory()
		{
			ViewBag.NavigationZone = NavigationZone.Games;
			return View();
		}
        
        
        [HttpGet("money-spin/{*extra}")]
		public ActionResult MoneySpin()
		{
			ViewBag.NavigationZone = NavigationZone.Games;
			return View();
		}
    }
}
