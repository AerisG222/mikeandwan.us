using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;


namespace MawAuth.ViewModels.Account
{
	public class ChangePasswordModel
	{
		[Required(ErrorMessage = "Please enter your current password")]
		[Display(Name = "Current Password")]
		[DataType(DataType.Password)]
		public string CurrentPassword { get; set; }

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
		public bool ChangeAttempted { get; set; }

		[BindNever]
		public bool ChangeSucceeded { get; set; }
	}
}

