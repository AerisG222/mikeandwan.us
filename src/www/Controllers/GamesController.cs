using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MawMvcApp.ViewModels.Navigation;


namespace MawMvcApp.Controllers
{
	[Route("games")]
    public class GamesController
		: MawBaseController<GamesController>
    {
		public GamesController(ILogger<GamesController> log)
			: base(log)
		{

		}


		[HttpGet("")]
        public IActionResult Index()
        {
			ViewBag.NavigationZone = NavigationZone.Games;
            return View ();
        }


		[HttpGet("memory/{*extra}")]
		public IActionResult Memory()
		{
			ViewBag.NavigationZone = NavigationZone.Games;
			return View();
		}


        [HttpGet("money-spin/{*extra}")]
		public IActionResult MoneySpin()
		{
			ViewBag.NavigationZone = NavigationZone.Games;
			return View();
		}
    }
}
