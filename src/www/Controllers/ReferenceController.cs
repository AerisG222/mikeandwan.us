using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MawMvcApp.ViewModels.Navigation;


namespace MawMvcApp.Controllers
{
	[Route("reference")]
    public class ReferenceController 
        : MawBaseController<ReferenceController>
    {
		public ReferenceController(ILogger<ReferenceController> log)
			: base(log)
		{

		}


		[HttpGet("")]
        public ActionResult Index()
        {
			ViewBag.NavigationZone = NavigationZone.Reference;
			
            return View();
        }
		
		
		[HttpGet("html-colors")]
		public ActionResult HtmlColors()
		{
			ViewBag.NavigationZone = NavigationZone.Reference;
			
			return View();
		}
		
		
		[HttpGet("html-doctypes")]
		public ActionResult HtmlDoctypes()
		{
			ViewBag.NavigationZone = NavigationZone.Reference;
			
			return View();
		}
		
		
		[HttpGet("html-entities")]
		public ActionResult HtmlEntities()
		{
			ViewBag.NavigationZone = NavigationZone.Reference;
			
			return View();
		}
		
		
		[HttpGet("http-status-codes")]
		public ActionResult HttpStatusCodes()
		{
			ViewBag.NavigationZone = NavigationZone.Reference;
			
			return View();
		}
		
		
		[HttpGet("http-rfcs")]
		public ActionResult HttpRfcs()
		{
			ViewBag.NavigationZone = NavigationZone.Reference;
			
			return View();
		}
		
		
		[HttpGet("networking-ports")]
		public ActionResult NetworkingPorts()
		{
			ViewBag.NavigationZone = NavigationZone.Reference;
			
			return View();
		}
		
		
		[HttpGet("dotnet-guidelines-fundamentals")]
		public ActionResult DotnetGuidelinesFundamentals()
		{
			ViewBag.NavigationZone = NavigationZone.Reference;
			
			return View();
		}
		
		
		[HttpGet("dotnet-guidelines-naming")]
		public ActionResult DotnetGuidelinesNaming()
		{
			ViewBag.NavigationZone = NavigationZone.Reference;
			
			return View();
		}
		
		
		[HttpGet("dotnet-guidelines-type-design")]
		public ActionResult DotnetGuidelinesTypeDesign()
		{
			ViewBag.NavigationZone = NavigationZone.Reference;
			
			return View();
		}
		
		
		[HttpGet("dotnet-guidelines-member-design")]
		public ActionResult DotnetGuidelinesMemberDesign()
		{
			ViewBag.NavigationZone = NavigationZone.Reference;
			
			return View();
		}
		
		
		[HttpGet("dotnet-guidelines-design-for-extensibility")]
		public ActionResult DotnetGuidelinesDesignForExtensibility()
		{
			ViewBag.NavigationZone = NavigationZone.Reference;
			
			return View();
		}
		
		
		[HttpGet("dotnet-guidelines-exceptions")]
		public ActionResult DotnetGuidelinesExceptions()
		{
			ViewBag.NavigationZone = NavigationZone.Reference;
			
			return View();
		}
		
		
		[HttpGet("dotnet-guidelines-usage-guidelines")]
		public ActionResult DotnetGuidelinesUsageGuidelines()
		{
			ViewBag.NavigationZone = NavigationZone.Reference;
			
			return View();
		}
		
		[HttpGet("dotnet-guidelines-common-patterns")]
		public ActionResult DotnetGuidelinesCommonPatterns()
		{
			ViewBag.NavigationZone = NavigationZone.Reference;
			
			return View();
		}
		
		[HttpGet("aspnet5")]
        public ActionResult Aspnet5()
        {
            ViewBag.NavigationZone = NavigationZone.Reference;
			
            return View();
        }
		
		[HttpGet("angular2")]
        public ActionResult Angular2()
        {
            ViewBag.NavigationZone = NavigationZone.Reference;
			
            return View();
        }
    }
}

