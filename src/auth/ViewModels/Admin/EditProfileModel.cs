using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;


namespace MawAuth.ViewModels.Admin
{
	public class EditProfileModel
	{
		[Required(ErrorMessage = "Please enter the username")]
		[Display(Name = "Username")]
		public string Username { get; set; }

		[Required(ErrorMessage = "Please enter the first name")]
		[Display(Name = "First Name")]
		public string FirstName { get; set; }

		[Required(ErrorMessage = "Please enter the last name")]
		[Display(Name = "Last Name")]
		public string LastName { get; set; }

		[Required(ErrorMessage = "Please enter the email")]
		[Display(Name = "Email")]
		[EmailAddress(ErrorMessage = "Please enter a valid email address")]
		[DataType(DataType.EmailAddress)]
		public string Email { get; set; }

		[BindNever]
		public IdentityResult Result { get; set; }
	}
}

