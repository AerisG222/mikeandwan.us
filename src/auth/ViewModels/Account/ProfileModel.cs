using System;
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
		
		[Required(ErrorMessage = "Please enter your date of birth")]
		[Display(Name = "Date of Birth")]
		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
		public DateTime? DateOfBirth { get; set; }
		
		[Display(Name = "Company")]
		public string Company { get; set; }
		
		[Display(Name = "Position")]
		public string Position { get; set; }
		
		[Required(ErrorMessage = "Please enter your email address")]
		[Display(Name = "Personal Email")]
		[EmailAddress(ErrorMessage = "Please enter a valid email address")]
		[DataType(DataType.EmailAddress)]
		public string PersonalEmail { get; set; }
		
		[Display(Name = "Work Email")]
		[EmailAddress(ErrorMessage = "Please enter a valid email address")]
		[DataType(DataType.EmailAddress)]
		public string WorkEmail { get; set; }
		
		[Display(Name = "Home Phone")]
		[DataType(DataType.PhoneNumber)]
		[Phone(ErrorMessage = "Please enter a valid phone number")]
		public string HomePhone { get; set; }
		
		[Display(Name = "Mobile Phone")]
		[DataType(DataType.PhoneNumber)]
		[Phone(ErrorMessage = "Please enter a valid phone number")]
		public string MobilePhone { get; set; }
		
		[Display(Name = "Work Phone")]
		[DataType(DataType.PhoneNumber)]
		[Phone(ErrorMessage = "Please enter a valid phone number")]
		public string WorkPhone { get; set; }
		
		[Display(Name = "Address Line 1")]
		public string Address1 { get; set; }
		
		[Display(Name = "Address Line 2")]
		public string Address2 { get; set; }
		
		[Display(Name = "City")]
		public string City { get; set; }
		
		[Display(Name = "State")]
		public string State { get; set; }
		
		[Display(Name = "Postal Code")]
		public string PostalCode { get; set; }
		
		[Display(Name = "Country")]
		public string Country { get; set; }
		
		[Display(Name = "Website")]
		[DataType(DataType.Url)]
		[Url(ErrorMessage = "Please enter a valid website address")]
		public string Website { get; set; }

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

