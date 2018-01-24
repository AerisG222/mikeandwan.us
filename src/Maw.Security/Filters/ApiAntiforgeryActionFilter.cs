using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;


namespace Maw.Security.Filters
{
    // TODO: this can go away once we only use bearer auth as there is no cookie that will automatically be sent / provide access
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ApiAntiforgeryActionFilter
        : ActionFilterAttribute
    {
        readonly IAntiforgery _antiforgery;


        public ApiAntiforgeryActionFilter(IAntiforgery antiforgery)
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

            // angular will translate xsrf-token cookie to x-xsrf-token header on subsequent calls
            var tokens = _antiforgery.GetAndStoreTokens(context.HttpContext);
            context.HttpContext.Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken, new CookieOptions()
                {
                    HttpOnly = false
                });

            if (context.Result == null)
            {
                OnActionExecuted(await next());
            }
        }
    }
}
