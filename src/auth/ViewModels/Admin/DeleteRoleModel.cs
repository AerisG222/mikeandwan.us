using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;


namespace MawAuth.ViewModels.Admin
{
	public class DeleteRoleModel
	{
		[Required]
		public string Role { get; set; }

		[BindNever]
		public IdentityResult Result { get; set; }
	}
}

