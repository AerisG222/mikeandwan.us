using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;


namespace MawMvcApp.ViewModels.Admin
{
	public class CreateRoleModel
	{
		[Required(ErrorMessage = "Please enter the name")]
		[Display(Name = "Role Name")]
		public string Name { get; set; }
		
		[Required(ErrorMessage = "Please enter the description")]
		[Display(Name = "Description")]
		public string Description { get; set; }
		
		[BindNever]
		public IdentityResult Result { get; set; }
	}
}

