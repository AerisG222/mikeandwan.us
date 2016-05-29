﻿using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;


namespace MawMvcApp.ViewModels.Account
{
	public class ResetPasswordModel
	{
		[Required(ErrorMessage = "Please enter your email address")]
		[EmailAddress(ErrorMessage = "Please enter a valid email address")]
		[DataType(DataType.EmailAddress)]
		public string Email { get; set; }

		[Required(ErrorMessage = "Please enter the code from the email")]
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
		public bool WasReset { get; set; }
	}
}

