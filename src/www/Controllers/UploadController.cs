using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Maw.Security;
using MawMvcApp.ViewModels.Navigation;


namespace MawMvcApp.Controllers
{
    [Authorize(Policy.CanUpload)]
	[Route("upload")]
    public class UploadController
        : MawBaseController<UploadController>
    {
		public UploadController(ILogger<UploadController> log)
			: base(log)
        {

        }


		[HttpGet("")]
        public IActionResult Index()
        {
			ViewBag.NavigationZone = NavigationZone.Upload;

            return View();
        }
    }
}
