using Microsoft.AspNetCore.Mvc;

namespace MawMvcApp.Controllers;

public class MawBaseController<T>
    : Controller
{
    protected ILogger Log { get; }

    public MawBaseController(ILogger<T> log)
    {
        ArgumentNullException.ThrowIfNull(log);

        Log = log;
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
