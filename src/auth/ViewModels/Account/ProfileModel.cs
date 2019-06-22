using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;


namespace MawAuth.ViewModels.Account
{
	public class ProfileModel
	{
		[Display(Name = "Username")]
		public string Username { get; set; }

		[Required(ErrorMessage = "Please enter your first name")]
		[Display(Name = "First Name")]
		public string FirstName { get; set; }

		[Required(ErrorMessage = "Please enter your last name")]
		[Display(Name = "Last Name")]
		public string LastName { get; set; }

		[Required(ErrorMessage = "Please enter your email address")]
		[Display(Name = "Email")]
		[EmailAddress(ErrorMessage = "Please enter a valid email address")]
		[DataType(DataType.EmailAddress)]
		public string Email { get; set; }

		[Display(Name = "GitHub")]
		public bool EnableGithubAuth { get; set; }

		[Display(Name = "Google")]
		public bool EnableGoogleAuth { get; set; }

		[Display(Name = "Microsoft")]
		public bool EnableMicrosoftAuth { get; set; }

		[Display(Name = "Twitter")]
		public bool EnableTwitterAuth { get; set; }

		[BindNever]
		public bool WasAttempted { get; set; }

		[BindNever]
		public bool WasUpdated { get; set; }
	}
}

