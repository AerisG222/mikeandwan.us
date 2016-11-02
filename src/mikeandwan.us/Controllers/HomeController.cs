using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MawMvcApp.ViewModels.Navigation;


namespace MawMvcApp.Controllers
{
    [Route("")]
    public class HomeController 
        : MawBaseController<HomeController>
    {
		public HomeController(ILogger<HomeController> log)
			: base(log)
		{

		}


        [HttpGet("")]
        public ActionResult Index()
        {
			ViewBag.NavigationZone = NavigationZone.Home;

            return View();
        }
    }
}
