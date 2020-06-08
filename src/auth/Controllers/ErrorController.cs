using System;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;


namespace MawAuth.Controllers
{
    [Route("error")]
    public class ErrorController
        : Controller
    {
        readonly ILogger _log;


        public ErrorController(ILogger<ErrorController> log)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }


        [HttpGet("")]
        public IActionResult Index()
        {
            var feature = HttpContext.Features.Get<IExceptionHandlerFeature>();

            _log.LogError("There was an error in the application: ", feature?.Error);
            _log.LogError("Inner Exception: ", feature?.Error?.InnerException);

            return View();
        }
    }
}

