using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using MawMvcApp.ViewModels;


namespace MawMvcApp.Controllers
{
    public class MawBaseController<T>
        : Controller
    {
		protected readonly IAuthorizationService _authzService;
		protected readonly ILogger<T> _log;
		

		public MawBaseController(IAuthorizationService authorizationService, 
		                         ILogger<T> log)
		{
			if(authorizationService == null)
			{
				throw new ArgumentNullException(nameof(authorizationService));
			}

			if(log == null)
			{
				throw new ArgumentNullException(nameof(log));
			}
			
			_authzService = authorizationService;
			_log = log; 
		}


		public async override void OnActionExecuted(ActionExecutedContext context)
		{
			ViewBag.AuthzPhoto = await _authzService.AuthorizeAsync(User, null, MawConstants.POLICY_VIEW_PHOTOS);
			ViewBag.AuthzVideo = await _authzService.AuthorizeAsync(User, null, MawConstants.POLICY_VIEW_VIDEOS);
			ViewBag.AuthzAdmin = await _authzService.AuthorizeAsync(User, null, MawConstants.POLICY_ADMIN_SITE);
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
