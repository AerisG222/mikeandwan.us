using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MawMvcApp.ViewModels.Navigation;

namespace MawMvcApp.Controllers;

[Route("error")]
public class ErrorController
    : MawBaseController<ErrorController>
{
    public ErrorController(ILogger<ErrorController> log)
        : base(log)
    {

    }

    [HttpGet("")]
    public IActionResult Index()
    {
        ViewBag.NavigationZone = NavigationZone.None;

        var feature = HttpContext.Features.Get<IExceptionHandlerFeature>();

        Log.LogError("There was an error in the application: ", feature?.Error);
        Log.LogError("Inner Exception: ", feature?.Error?.InnerException);

        return View();
    }
}
