using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;


namespace MawMvcApp.ViewModels.Account
{
	public class ChangePasswordModel
		: IValidatableObject
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


		public IEnumerable<ValidationResult> Validate (ValidationContext validationContext)
		{
			if(NewPassword.Length < 6 || NewPassword.Length > 30)
			{
				yield return new ValidationResult("Your new password must be between 6 and 30 characters in length", new string[] { nameof(NewPassword) });
			}
		}
	}
}

