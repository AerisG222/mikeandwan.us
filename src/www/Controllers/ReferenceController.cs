using Microsoft.AspNetCore.Mvc;
using MawMvcApp.ViewModels.Navigation;

namespace MawMvcApp.Controllers;

[Route("reference")]
public class ReferenceController
    : MawBaseController<ReferenceController>
{
    public ReferenceController(ILogger<ReferenceController> log)
        : base(log)
    {

    }

    [HttpGet("")]
    public IActionResult Index()
    {
        ViewBag.NavigationZone = NavigationZone.Reference;

        return View();
    }

    [HttpGet("html-colors")]
    public IActionResult HtmlColors()
    {
        ViewBag.NavigationZone = NavigationZone.Reference;

        return View();
    }

    [HttpGet("html-doctypes")]
    public IActionResult HtmlDoctypes()
    {
        ViewBag.NavigationZone = NavigationZone.Reference;

        return View();
    }

    [HttpGet("html-entities")]
    public IActionResult HtmlEntities()
    {
        ViewBag.NavigationZone = NavigationZone.Reference;

        return View();
    }

    [HttpGet("http-status-codes")]
    public IActionResult HttpStatusCodes()
    {
        ViewBag.NavigationZone = NavigationZone.Reference;

        return View();
    }

    [HttpGet("http-rfcs")]
    public IActionResult HttpRfcs()
    {
        ViewBag.NavigationZone = NavigationZone.Reference;

        return View();
    }

    [HttpGet("dotnet-guidelines-fundamentals")]
    public IActionResult DotnetGuidelinesFundamentals()
    {
        ViewBag.NavigationZone = NavigationZone.Reference;

        return View();
    }

    [HttpGet("dotnet-guidelines-naming")]
    public IActionResult DotnetGuidelinesNaming()
    {
        ViewBag.NavigationZone = NavigationZone.Reference;

        return View();
    }

    [HttpGet("dotnet-guidelines-type-design")]
    public IActionResult DotnetGuidelinesTypeDesign()
    {
        ViewBag.NavigationZone = NavigationZone.Reference;

        return View();
    }

    [HttpGet("dotnet-guidelines-member-design")]
    public IActionResult DotnetGuidelinesMemberDesign()
    {
        ViewBag.NavigationZone = NavigationZone.Reference;

        return View();
    }

    [HttpGet("dotnet-guidelines-design-for-extensibility")]
    public IActionResult DotnetGuidelinesDesignForExtensibility()
    {
        ViewBag.NavigationZone = NavigationZone.Reference;

        return View();
    }

    [HttpGet("dotnet-guidelines-exceptions")]
    public IActionResult DotnetGuidelinesExceptions()
    {
        ViewBag.NavigationZone = NavigationZone.Reference;

        return View();
    }

    [HttpGet("dotnet-guidelines-usage-guidelines")]
    public IActionResult DotnetGuidelinesUsageGuidelines()
    {
        ViewBag.NavigationZone = NavigationZone.Reference;

        return View();
    }

    [HttpGet("dotnet-guidelines-common-patterns")]
    public IActionResult DotnetGuidelinesCommonPatterns()
    {
        ViewBag.NavigationZone = NavigationZone.Reference;

        return View();
    }
}
