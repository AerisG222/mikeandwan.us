using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace MawAuth.Controllers;

[Route("error")]
public class ErrorController
    : Controller
{
    readonly ILogger _log;

    public ErrorController(ILogger<ErrorController> log)
    {
        ArgumentNullException.ThrowIfNull(log);

        _log = log;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
        var feature = HttpContext.Features.Get<IExceptionHandlerFeature>();

        _log.LogError("There was an error in the application: {Error}", feature?.Error);
        _log.LogError("Inner Exception: {Error}", feature?.Error?.InnerException);

        return View();
    }
}
