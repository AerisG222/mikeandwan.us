using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;


namespace MawMvcApp.Controllers
{
    public class MawBaseController<T>
        : Controller
    {
		protected readonly ILogger<T> _log;
		

		public MawBaseController(ILogger<T> log)
		{
			if(log == null)
			{
				throw new ArgumentNullException(nameof(log));
			}
			
			_log = log; 
		}
		
		
		protected void LogValidationErrors()
		{
			var errs = ModelState.Values.SelectMany(v => v.Errors);
				
			foreach (var err in errs)
			{
				_log.LogWarning(err.ErrorMessage);
			}
		}
    }
}
