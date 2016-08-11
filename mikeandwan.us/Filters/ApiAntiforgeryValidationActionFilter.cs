using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Antiforgery;


namespace MawMvcApp.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ApiAntiforgeryValidationActionFilter 
        : ActionFilterAttribute
    {
        readonly IAntiforgery _antiforgery;


        public ApiAntiforgeryValidationActionFilter(IAntiforgery antiforgery)
        {
            if(antiforgery == null)
            {
                throw new ArgumentNullException(nameof(antiforgery));
            }

            _antiforgery = antiforgery;
        }


        public override async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            await _antiforgery.ValidateRequestAsync(context.HttpContext);

            if (context.Result == null)
            {
                OnActionExecuted(await next());
            }
        }
    }
}
