using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using MawMvcApp.ViewModels.Gps;
using MawMvcApp.ViewModels.Navigation;
using MawMvcApp.ViewModels.Tools;
using MawMvcApp.ViewModels.Tools.Dotnet;


namespace MawMvcApp.Controllers
{
	[Route("tools")]
    public class ToolsController 
        : MawBaseController<ToolsController>
    {
		IFileProvider _fileProvider { get; set; }


		public ToolsController(ILogger<ToolsController> log, 
							   IHostingEnvironment env)
			: base(log)
        {
			if(env == null)
			{
				throw new ArgumentNullException(nameof(env));
			}

			_fileProvider = env.WebRootFileProvider;
        }


		[HttpGet("")]
        public ActionResult Index()
        {
			ViewBag.NavigationZone = NavigationZone.Tools;

            return View();
        }
		
		
		[HttpGet("roll-the-dice")]
		public ActionResult RollTheDice()
        {
			ViewBag.NavigationZone = NavigationZone.Tools;
			
			var model = new RollTheDiceModel 
			{ 
				NumberOfThrows = 10, 
				NumberOfSides = 6 
			};
				
            return View(model);
        }
		
		
		[HttpPost("roll-the-dice")]
		[ValidateAntiForgeryToken]
		public ActionResult RollTheDice(RollTheDiceModel model)
		{
			ViewBag.NavigationZone = NavigationZone.Tools;
			
			if(ModelState.IsValid)
			{
				model.ThrowDice();
			}
			else
			{
				LogValidationErrors();
			}
			
			return View(model);
		}
		
		
		[HttpGet("google-maps")]
		public ActionResult GoogleMaps()
		{
			ViewBag.NavigationZone = NavigationZone.Tools;
			
			return View();
		}
		
		
		[HttpGet("binary-clock-about")]
		public ActionResult BinaryClockAbout()
		{
			ViewBag.NavigationZone = NavigationZone.Tools;
			
			return View();
		}
		
		
		[HttpGet("binary-clock")]
		public ActionResult BinaryClock()
		{
			ViewBag.NavigationZone = NavigationZone.Tools;
			
			return View();
		}
		
		
		[HttpGet("dotnet-formatting-dates")]
		public ActionResult DotnetFormattingDates()
		{
			var mgr = new FormatExampleManager();
            var date = new DateTime(1977, 10, 3, 3, 24, 46, 789, DateTimeKind.Local);
			
			ViewBag.NavigationZone = NavigationZone.Tools;
			ViewBag.ExampleDate = date;
			
			return View(mgr.GetDateFormatExamples(date));
		}
		
		
		[HttpGet("dotnet-formatting-numbers")]
		public ActionResult DotnetFormattingNumbers()
		{
			var mgr = new FormatExampleManager();
			double value = 1234.125678;
			
			ViewBag.NavigationZone = NavigationZone.Tools;
			ViewBag.ExampleDate = value;
			
			return View(mgr.GetNumberFormatExamples(value));
		}
		
		
		[HttpGet("dotnet-regex")]
		public ActionResult DotnetRegex()
		{
			ViewBag.NavigationZone = NavigationZone.Tools;
			
			return View(new RegexViewModel());
		}
		
		
		[HttpPost("dotnet-regex")]
		[ValidateAntiForgeryToken]
		public ActionResult DotnetRegex(RegexViewModel model)
		{
			ViewBag.NavigationZone = NavigationZone.Tools;
			
			if(ModelState.IsValid)
			{
				model.Execute();
			}
			else
			{
				LogValidationErrors();
			}
			
			return View(model);
		}
		
		
		[HttpGet("networking-bandwidth")]
		public ActionResult NetworkingBandwidth()
		{
			ViewBag.NavigationZone = NavigationZone.Tools;
			
			return View();
		}
		
		
		[HttpGet("networking-file-size")]
		public ActionResult NetworkingFileSize()
		{
			ViewBag.NavigationZone = NavigationZone.Tools;
			
			return View();
		}
		
		
		[HttpGet("networking-time")]
		public ActionResult NetworkingTime()
		{
			ViewBag.NavigationZone = NavigationZone.Tools;
			
			return View();
		}
		
		
		[HttpGet("weekend-countdown")]
		public ActionResult WeekendCountdown()
		{
			ViewBag.NavigationZone = NavigationZone.Tools;
			
			return View();
		}
		
		
		[HttpGet("weather")]
		public ActionResult Weather()
		{
			ViewBag.NavigationZone = NavigationZone.Tools;
			
			return View();
		}
		
		
		[HttpGet("browser-hell")]
		public ActionResult BrowserHell()
		{
			ViewBag.NavigationZone = NavigationZone.Tools;
			
            var model = new BrowserHellModel();
            var fi = _fileProvider.GetFileInfo("img/tools/browser_hell.jpg");

            if(fi.Exists)
            {
                ViewBag.ImageColors = model.GetColorArray(fi.PhysicalPath);
			
			    return View();
            }

            return NotFound();
		}
		
		
		[HttpGet("byte-counter")]
		public ActionResult ByteCounter()
		{
			ViewBag.NavigationZone = NavigationZone.Tools;
			
			return View();
		}
		
		
		[HttpGet("date-diff")]
		public ActionResult DateDiff()
		{
			ViewBag.NavigationZone = NavigationZone.Tools;
			
            var model = new DateDiff 
			{ 
				StartDate = DateTime.Now.AddYears(-1), 
				EndDate = DateTime.Now 
			};

			return View(model);
		}
		
		
		[HttpPost("date-diff")]
		[ValidateAntiForgeryToken]
		public ActionResult DateDiff(DateDiff dd)
		{
			ViewBag.NavigationZone = NavigationZone.Tools;
			
			dd.ShowResults = ModelState.IsValid;
			
			if(!ModelState.IsValid)
			{
				LogValidationErrors();
			}
			
			return View(dd);
		}
		
		
		[HttpGet("gps-conversion")]
		public ActionResult GpsConversion()
		{
			ViewBag.NavigationZone = NavigationZone.Tools;

			return View(new GpsConversionModel());
		}
		
		
		[HttpPost("gps-conversion")]
		[ValidateAntiForgeryToken]
		public ActionResult GpsConversion(GpsConversionModel model)
		{
			ViewBag.NavigationZone = NavigationZone.Tools;
			
			if(ModelState.IsValid)
			{
				model.Convert();
			}
			else
			{
				LogValidationErrors();
			}
			
			return View(model);
		}
		
		
		[HttpGet("guid-gen")]
		public ActionResult GuidGen()
		{
			ViewBag.NavigationZone = NavigationZone.Tools;
			ViewBag.Guid = Guid.NewGuid().ToString();
			
			return View();
		}
		
		
		[HttpGet("html-encode")]
		public ActionResult HtmlEncode()
		{
			ViewBag.NavigationZone = NavigationZone.Tools;
			
			return View(new HtmlEncodeDecodeModel());
		}
		
		
		[HttpPost("html-encode")]
		[ValidateAntiForgeryToken]
		public ActionResult HtmlEncode(HtmlEncodeDecodeModel model)
		{
			ViewBag.NavigationZone = NavigationZone.Tools;
			
			if(ModelState.IsValid)
			{
				if(model.Mode == EncodeMode.Decode)
				{
					model.Decode();
				}
				else
				{
					model.Encode();
				}
			}
			else
			{
				LogValidationErrors();
			}
			
			return View(model);
		}
		
		
		[HttpGet("paths")]
		public ActionResult Paths()
		{
			ViewBag.NavigationZone = NavigationZone.Tools;
			
			var model = new PathsModel();
			model.PreparePaths();
			
			return View(model);
		}
		
		
		[HttpGet("random-bytes")]
		public ActionResult RandomBytes()
		{
			ViewBag.NavigationZone = NavigationZone.Tools;
			
			return View(new RandomBytesModel());
		}
		
		
		[HttpPost("random-bytes")]
		[ValidateAntiForgeryToken]
		public ActionResult RandomBytes(RandomBytesModel model)
		{
			ViewBag.NavigationZone = NavigationZone.Tools;
			
			if(ModelState.IsValid)
			{
				model.GenerateRandomness();
			}
			else
			{
				LogValidationErrors();
			}
			
			return View(model);
		}
		

        [HttpGet("color-converter")]
        public ActionResult ColorConverter()
        {
            ViewBag.NavigationZone = NavigationZone.Tools;

            return View(new ColorConverterModel());
        }


        [HttpPost("color-converter")]
		[ValidateAntiForgeryToken]
        public ActionResult ColorConverter(ColorConverterModel model)
        {
            ViewBag.NavigationZone = NavigationZone.Tools;

            if(ModelState.IsValid)
            {
                model.Convert();
            }
			else
			{
				LogValidationErrors();
			}

            return View(model);
        }

		
		[HttpGet("url-encode")]
		public ActionResult UrlEncode()
		{
			ViewBag.NavigationZone = NavigationZone.Tools;
			
			return View(new UrlEncodeModel());
		}
		
		
		[HttpPost("url-encode")]
		[ValidateAntiForgeryToken]
		public ActionResult UrlEncode(UrlEncodeModel model)
		{
			ViewBag.NavigationZone = NavigationZone.Tools;
			
			if(ModelState.IsValid)
			{
				model.PerformCoding();
			}
			else
			{
				LogValidationErrors();
			}
			
			return View(model);
		}
		

#if NET451
		[HttpGet("xml-validate")]
		public ActionResult XmlValidate()
		{
			ViewBag.NavigationZone = NavigationZone.Tools;
			
			return View(new XmlValidateModel());
		}
		
		
		[HttpPost("xml-validate")]
		[ValidateAntiForgeryToken]
		public ActionResult XmlValidate(XmlValidateModel model)
		{
			ViewBag.NavigationZone = NavigationZone.Tools;
			
			if(ModelState.IsValid)
			{
				model.ValidateXml();
			}
			else
			{
				LogValidationErrors();
			}
			
			return View(model);
		}
		
		
		[HttpGet("xsd-validate")]
		public ActionResult XsdValidate()
		{
			ViewBag.NavigationZone = NavigationZone.Tools;
			
			return View(new XsdValidateModel());
		}
		
		
		[HttpPost("xsd-validate")]
		[ValidateAntiForgeryToken]
		public ActionResult XsdValidate(XsdValidateModel model)
		{
			ViewBag.NavigationZone = NavigationZone.Tools;
			
			if(ModelState.IsValid)
			{
				model.ValidateSchema();
			}
			else
			{
				LogValidationErrors();
			}
			
			return View(model);
		}
		
		
		[HttpGet("xsl-transform")]
		public ActionResult XslTransform()
		{
			ViewBag.NavigationZone = NavigationZone.Tools;
			
			return View(new XslTransformModel());
		}
		
		
		[HttpPost("xsl-transform")]
		[ValidateAntiForgeryToken]
		public ActionResult XslTransform(XslTransformModel model)
		{
			ViewBag.NavigationZone = NavigationZone.Tools;
			
			if(ModelState.IsValid)
			{
				model.ExecuteTransform();
			}
			else
			{
				LogValidationErrors();
			}
			
			return View(model);
		}
#endif


        [HttpGet("learning")]
        public ActionResult Learning()
        {
            ViewBag.NavigationZone = NavigationZone.Tools;

            return View();
        }
    }
}

