using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;


namespace MawMvcApp.ViewModels.Account
{
	public class LoginModel
	{
		[Required(ErrorMessage = "Please enter your username")]
		[Display(Name = "Username")]
		public string Username { get; set; }

		[Required(ErrorMessage = "Please enter your password")]
		[Display(Name = "Password")]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		//[BindNever]
		//public IEnumerable<ExternalLoginScheme> ExternalSchemes { get; set; }

		[BindNever]
		public bool WasAttempted { get; set; }
	}
}

