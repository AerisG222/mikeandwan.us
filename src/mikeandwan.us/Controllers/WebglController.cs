using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MawMvcApp.ViewModels.Navigation;


namespace MawMvcApp.Controllers
{
    [Route("webgl")]
    public class WebglController 
		: MawBaseController<WebglController>
    {
		public WebglController(ILogger<WebglController> log)
			: base(log)
		{

		}


        [HttpGet("")]
        public ActionResult Index()
        {
            ViewBag.NavigationZone = NavigationZone.Webgl;
            return View();
        }
        
        
        [HttpGet("cube")]
        public ActionResult Cube()
        {
            ViewBag.NavigationZone = NavigationZone.Webgl;
            return View();
        }
        
        
        [HttpGet("text")]
        public ActionResult Text()
        {
            ViewBag.NavigationZone = NavigationZone.Webgl;
            return View();
        }
    }
}
