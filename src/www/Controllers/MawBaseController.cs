using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;


namespace MawMvcApp.Controllers
{
    public class MawBaseController<T>
        : Controller
    {
        protected ILogger Log { get; }


        public MawBaseController(ILogger<T> log)
        {
            Log = log ?? throw new ArgumentNullException(nameof(log));
        }


        protected void LogValidationErrors()
        {
            var errs = ModelState.Values.SelectMany(v => v.Errors);

            foreach (var err in errs)
            {
                Log.LogWarning("validation error: {ValidationError}", err.ErrorMessage);
            }
        }
    }
}
