/*
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Antiforgery;


namespace MawMvcApp.ViewModels
{
	// TODO: looks like xsrf stuff is in flux, and an improved approach might be coming to the framework, reference:
	// https://github.com/aspnet/Mvc/issues/2619
	// https://github.com/aspnet/Mvc/issues/2618
	public class AngularXSRF
	{
		const string ANGULAR_COOKIE_NAME = "XSRF-TOKEN";
		const string HTTPONLY_COOKIE_NAME = "XSRF-TOKEN-HTTP";
		 
		readonly HttpContext _context;
		readonly IAntiforgery _antiForgery;
		
		
		// the angular cookie is available to javascript, and is equivalent to the hidden form input
		// in the mvc xsrf tooling.
		string AngularXsrfCookieValue
		{
			get 
			{ 
				return GetCookie(ANGULAR_COOKIE_NAME); 
			}
			set
			{
				var opt = new CookieOptions();
				opt.HttpOnly = false;
				
				if (!IsIdenticalCookie(ANGULAR_COOKIE_NAME, value))
				{
					_context.Response.Cookies.Delete(ANGULAR_COOKIE_NAME);
				}
				
				_context.Response.Cookies.Append(ANGULAR_COOKIE_NAME, value, opt);
			}
		}
		
		
		// this is the http only xsrf cookie that is not available to javascript and will be sent with 
		// subsequent requests.  this is the matching pair for angularxsrfcookievalue
		string HttpXsrfCookieValue
		{
			get 
			{ 
				return GetCookie(HTTPONLY_COOKIE_NAME); 
			}
			set
			{
				if (!IsIdenticalCookie(HTTPONLY_COOKIE_NAME, value))
				{
					_context.Response.Cookies.Delete(HTTPONLY_COOKIE_NAME);
				}
				
				_context.Response.Cookies.Append(HTTPONLY_COOKIE_NAME, value);
			}
		}
		
		
		public AngularXSRF(HttpContext context, IAntiforgery antiForgery)
		{
			if (context == null)
			{
				throw new ArgumentNullException(nameof(context));
			}
			
			if(antiForgery == null)
			{
				throw new ArgumentNullException(nameof(antiForgery));
			}
			
			_context = context;
			_antiForgery = antiForgery;
		}
		
		
		public void GenerateTokens()
		{
			var tokens = _antiForgery.GetTokens(_context);
			
			AngularXsrfCookieValue = tokens.FormToken;
			HttpXsrfCookieValue = tokens.CookieToken;
		}
		
		
		public bool ValidateTokens()
		{
			return false;
		}
		
		
		string GetCookie(string cookieName)
		{
			if(_context.Request.Cookies.ContainsKey(cookieName))
			{
				//return _context.Request.Cookies.Get(cookieName);
			}
			
			return null;
		}
		
		
		bool IsIdenticalCookie(string cookieName, string value)
		{
			var currValue = GetCookie(cookieName);
			
			if (currValue == null || !string.Equals(currValue, value, StringComparison.Ordinal))
			{
				return false;
			}
			
			return true;
		}
	}
}
*/