using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;


namespace MawMvcApp.ViewModels.About
{
	public class ContactModel
	{
		public string RecaptchaSiteKey { get; set; }


		[Required(ErrorMessage = "Please enter your email address")]
		[Display(Name = "Email")]
		[EmailAddress(ErrorMessage = "Please enter a valid email address")]
		[DataType(DataType.EmailAddress)]
		public string Email { get; set; }
		
		[Required(ErrorMessage = "Please enter your first name")]
		[Display(Name = "First Name")]
		public string FirstName { get; set; }
		
		[Required(ErrorMessage = "Please enter your last name")]
		[Display(Name = "Last Name")]
		public string LastName { get; set; }
		
		[Required(ErrorMessage = "Please enter your message")]
		[DataType(DataType.MultilineText)]
		[Display(Name = "Message")]
		public string Message { get; set; }

		[BindNever]
		public bool IsHuman { get; set; }
		
		[BindNever]
		public bool SubmitAttempted { get; set; }
		
		[BindNever]
		public bool SubmitSuccess { get; set; }
	}
}

