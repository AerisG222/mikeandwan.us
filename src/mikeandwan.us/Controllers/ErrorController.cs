using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MawMvcApp.ViewModels.Navigation;


namespace MawMvcApp.Controllers
{
	[Route("error")]
	public class ErrorController
        : MawBaseController<ErrorController>
	{
		public ErrorController(ILogger<ErrorController> log)
			: base(log)
		{

		}


		[HttpGet("")]
		public ActionResult Index()
        {
			ViewBag.NavigationZone = NavigationZone.None;

			var feature = HttpContext.Features.Get<IExceptionHandlerFeature>();

            _log.LogError("There was an error in the application: ", feature?.Error);
			_log.LogError("Inner Exception: ", feature?.Error?.InnerException);

			return View();
        }
	}
}

