using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Maw.Security;


namespace MawMvcApp.Controllers
{
    [Authorize(MawPolicy.CanUpload)]
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
			return Redirect("https://files.mikeandwan.us");
        }
    }
}
