using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;


namespace MawAuth.ViewModels.Account
{
	public class ResetPasswordModel
	{
		[Required(ErrorMessage = "Please enter your email address")]
		[EmailAddress(ErrorMessage = "Please enter a valid email address")]
		[DataType(DataType.EmailAddress)]
		public string Email { get; set; }

		[Required(ErrorMessage = "Reset code was not accepted.  Please use the link from the reset password email.")]
		public string Code { get; set; }

		[Required(ErrorMessage = "Please enter your new password")]
		[Display(Name = "New Password")]
		[DataType(DataType.Password)]
		public string NewPassword { get; set; }

		[Required(ErrorMessage = "Please re-enter your new password")]
		[Display(Name = "Confirm New Password")]
		[DataType(DataType.Password)]
		[Compare(nameof(NewPassword), ErrorMessage = "Passwords did not match")]
		public string ConfirmPassword { get; set; }

		[BindNever]
		public bool ResetAttempted { get; set; }

		[BindNever]
		public bool WasReset { get; set; }
	}
}

