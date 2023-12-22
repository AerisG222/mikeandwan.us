using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using MawMvcApp.ViewModels.Gps;
using MawMvcApp.ViewModels.Navigation;
using MawMvcApp.ViewModels.Tools;
using MawMvcApp.ViewModels.Tools.Dotnet;
using MawMvcApp.ViewModels.Tools.Bandwidth;
using MawMvcApp.ViewModels.Tools.FileSize;
using MawMvcApp.ViewModels.Tools.Time;

namespace MawMvcApp.Controllers;

[Route("tools")]
public class ToolsController
    : MawBaseController<ToolsController>
{
    readonly IFileProvider _fileProvider;

    public ToolsController(
        ILogger<ToolsController> log,
        IWebHostEnvironment env)
        : base(log)
    {
        ArgumentNullException.ThrowIfNull(env?.WebRootFileProvider);

        _fileProvider = env?.WebRootFileProvider!;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
        ViewBag.NavigationZone = NavigationZone.Tools;

        return View();
    }

    [HttpGet("roll-the-dice")]
    public IActionResult RollTheDice()
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
    public IActionResult RollTheDice(RollTheDiceModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        ViewBag.NavigationZone = NavigationZone.Tools;

        if (ModelState.IsValid)
        {
            model.ThrowDice();
        }
        else
        {
            model.HasErrors = true;
            LogValidationErrors();
        }

        return View(model);
    }

    [HttpGet("google-maps")]
    public IActionResult GoogleMaps()
    {
        ViewBag.NavigationZone = NavigationZone.Tools;

        return View();
    }

    [HttpGet("binary-clock-about")]
    public IActionResult BinaryClockAbout()
    {
        ViewBag.NavigationZone = NavigationZone.Tools;

        return View();
    }

    [HttpGet("binary-clock")]
    public IActionResult BinaryClock()
    {
        ViewBag.NavigationZone = NavigationZone.Tools;

        return View();
    }

    [HttpGet("dotnet-formatting-dates")]
    public IActionResult DotnetFormattingDates()
    {
        var mgr = new FormatExampleManager();
        var date = new DateTime(1977, 10, 3, 3, 24, 46, 789, DateTimeKind.Local);

        ViewBag.NavigationZone = NavigationZone.Tools;
        ViewBag.ExampleDate = date;

        return View(mgr.GetDateFormatExamples(date));
    }

    [HttpGet("dotnet-formatting-numbers")]
    public IActionResult DotnetFormattingNumbers()
    {
        var mgr = new FormatExampleManager();
        double value = 1234.125678;

        ViewBag.NavigationZone = NavigationZone.Tools;
        ViewBag.ExampleDate = value;

        return View(mgr.GetNumberFormatExamples(value));
    }

    [HttpGet("dotnet-regex")]
    public IActionResult DotnetRegex()
    {
        ViewBag.NavigationZone = NavigationZone.Tools;

        return View(new RegexViewModel());
    }

    [HttpPost("dotnet-regex")]
    [ValidateAntiForgeryToken]
    public IActionResult DotnetRegex(RegexViewModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        ViewBag.NavigationZone = NavigationZone.Tools;

        if (ModelState.IsValid)
        {
            model.Execute();

            if(model.InvalidRegexOptions)
            {
                ModelState.AddModelError("options", "Invalid combination of RegEx options (most likely due to ECMA Script)");
            }
        }
        else
        {
            model.HasErrors = true;
            LogValidationErrors();
        }

        return View(model);
    }

    [HttpGet("networking-bandwidth")]
    public IActionResult NetworkingBandwidth()
    {
        ViewBag.NavigationZone = NavigationZone.Tools;

        return View(new BandwidthViewModel());
    }

    [HttpPost("networking-bandwidth")]
    [ValidateAntiForgeryToken]
    public IActionResult NetworkingBandwidth(BandwidthViewModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        ViewBag.NavigationZone = NavigationZone.Tools;

        if (ModelState.IsValid)
        {
            model.Calculate();
        }
        else
        {
            model.ErrorMessage = "Please enter a number for the file size";
        }

        return View(model);
    }

    [HttpGet("networking-file-size")]
    public IActionResult NetworkingFileSize()
    {
        ViewBag.NavigationZone = NavigationZone.Tools;

        return View(new FileSizeViewModel());
    }

    [HttpPost("networking-file-size")]
    [ValidateAntiForgeryToken]
    public IActionResult NetworkingFileSize(FileSizeViewModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        ViewBag.NavigationZone = NavigationZone.Tools;

        if (ModelState.IsValid)
        {
            model.Calculate();
        }
        else
        {
            model.ErrorMessage = "Please enter a valid file size";
        }

        return View(model);
    }

    [HttpGet("networking-time")]
    public IActionResult NetworkingTime()
    {
        ViewBag.NavigationZone = NavigationZone.Tools;

        return View(new NetworkingTimeViewModel());
    }

    [HttpPost("networking-time")]
    [ValidateAntiForgeryToken]
    public IActionResult NetworkingTime(NetworkingTimeViewModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        ViewBag.NavigationZone = NavigationZone.Tools;

        if (ModelState.IsValid)
        {
            model.Calculate();
        }
        else
        {
            model.ErrorMessage = "Please enter a valid time";
        }

        return View(model);
    }

    [HttpGet("weekend-countdown")]
    public IActionResult WeekendCountdown()
    {
        ViewBag.NavigationZone = NavigationZone.Tools;

        return View();
    }

    [HttpGet("weather")]
    public IActionResult Weather()
    {
        ViewBag.NavigationZone = NavigationZone.Tools;

        return View();
    }

    [HttpGet("byte-counter")]
    public IActionResult ByteCounter()
    {
        ViewBag.NavigationZone = NavigationZone.Tools;

        return View(new ByteCounterViewModel());
    }

    [HttpPost("byte-counter")]
    [ValidateAntiForgeryToken]
    public IActionResult ByteCounter(ByteCounterViewModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        ViewBag.NavigationZone = NavigationZone.Tools;

        model.Calculate();

        return View(model);
    }

    [HttpGet("date-diff")]
    public IActionResult DateDiff()
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
    public IActionResult DateDiff(DateDiff model)
    {
        ArgumentNullException.ThrowIfNull(model);

        ViewBag.NavigationZone = NavigationZone.Tools;

        model.ShowResults = ModelState.IsValid;

        if (!ModelState.IsValid)
        {
            model.HasErrors = true;
            LogValidationErrors();
        }

        return View(model);
    }

    [HttpGet("gps-conversion")]
    public IActionResult GpsConversion()
    {
        ViewBag.NavigationZone = NavigationZone.Tools;

        return View(new GpsConversionModel());
    }

    [HttpPost("gps-conversion")]
    [ValidateAntiForgeryToken]
    public IActionResult GpsConversion(GpsConversionModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        ViewBag.NavigationZone = NavigationZone.Tools;

        if (ModelState.IsValid)
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
    public IActionResult GuidGen()
    {
        ViewBag.NavigationZone = NavigationZone.Tools;
        ViewBag.Guid = Guid.NewGuid().ToString();

        return View();
    }

    [HttpGet("html-encode")]
    public IActionResult HtmlEncode()
    {
        ViewBag.NavigationZone = NavigationZone.Tools;

        return View(new HtmlEncodeDecodeModel());
    }

    [HttpPost("html-encode")]
    [ValidateAntiForgeryToken]
    public IActionResult HtmlEncode(HtmlEncodeDecodeModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        ViewBag.NavigationZone = NavigationZone.Tools;

        if (ModelState.IsValid)
        {
            if (model.Mode == EncodeMode.Decode)
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
            model.HasErrors = true;
            LogValidationErrors();
        }

        return View(model);
    }

    [HttpGet("paths")]
    public IActionResult Paths()
    {
        ViewBag.NavigationZone = NavigationZone.Tools;

        var model = new PathsModel();
        model.PreparePaths();

        return View(model);
    }

    [HttpGet("random-bytes")]
    public IActionResult RandomBytes()
    {
        ViewBag.NavigationZone = NavigationZone.Tools;

        return View(new RandomBytesModel());
    }

    [HttpPost("random-bytes")]
    [ValidateAntiForgeryToken]
    public IActionResult RandomBytes(RandomBytesModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        ViewBag.NavigationZone = NavigationZone.Tools;

        if (ModelState.IsValid)
        {
            model.GenerateRandomness();
        }
        else
        {
            model.HasErrors = true;
            LogValidationErrors();
        }

        return View(model);
    }

    [HttpGet("color-converter")]
    public IActionResult ColorConverter()
    {
        ViewBag.NavigationZone = NavigationZone.Tools;

        return View(new ColorConverterModel());
    }

    [HttpPost("color-converter")]
    [ValidateAntiForgeryToken]
    public IActionResult ColorConverter(ColorConverterModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        ViewBag.NavigationZone = NavigationZone.Tools;

        if (ModelState.IsValid)
        {
            model.Convert();
        }
        else
        {
            model.HasErrors = true;
            LogValidationErrors();
        }

        return View(model);
    }

    [HttpGet("url-encode")]
    public IActionResult UrlEncode()
    {
        ViewBag.NavigationZone = NavigationZone.Tools;

        return View(new UrlEncodeModel());
    }

    [HttpPost("url-encode")]
    [ValidateAntiForgeryToken]
    public IActionResult UrlEncode(UrlEncodeModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        ViewBag.NavigationZone = NavigationZone.Tools;

        if (ModelState.IsValid)
        {
            model.PerformCoding();
        }
        else
        {
            model.HasErrors = true;
            LogValidationErrors();
        }

        return View(model);
    }

    [HttpGet("xml-validate")]
    public IActionResult XmlValidate()
    {
        ViewBag.NavigationZone = NavigationZone.Tools;

        return View(new XmlValidateModel());
    }

    [HttpPost("xml-validate")]
    [ValidateAntiForgeryToken]
    public IActionResult XmlValidate(XmlValidateModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        ViewBag.NavigationZone = NavigationZone.Tools;

        if (ModelState.IsValid)
        {
            model.ValidateXml();
        }
        else
        {
            model.HasErrors = true;
            LogValidationErrors();
        }

        return View(model);
    }

    [HttpGet("xsd-validate")]
    public IActionResult XsdValidate()
    {
        ViewBag.NavigationZone = NavigationZone.Tools;

        return View(new XsdValidateModel());
    }

    [HttpPost("xsd-validate")]
    [ValidateAntiForgeryToken]
    public IActionResult XsdValidate(XsdValidateModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        ViewBag.NavigationZone = NavigationZone.Tools;

        if (ModelState.IsValid)
        {
            model.ValidateSchema();
        }
        else
        {
            model.HasErrors = true;
            LogValidationErrors();
        }

        return View(model);
    }

    [HttpGet("xsl-transform")]
    public IActionResult XslTransform()
    {
        ViewBag.NavigationZone = NavigationZone.Tools;

        return View(new XslTransformModel());
    }

    [HttpPost("xsl-transform")]
    [ValidateAntiForgeryToken]
    public IActionResult XslTransform(XslTransformModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        ViewBag.NavigationZone = NavigationZone.Tools;

        if (ModelState.IsValid)
        {
            model.ExecuteTransform();
        }
        else
        {
            model.HasErrors = true;
            LogValidationErrors();
        }

        return View(model);
    }

    [HttpGet("learning")]
    public IActionResult Learning()
    {
        ViewBag.NavigationZone = NavigationZone.Tools;

        return View();
    }
}
