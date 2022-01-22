using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MawAuth.ViewModels.Admin;

public class CreateRoleModel
{
    [Required(ErrorMessage = "Please enter the name")]
    [Display(Name = "Role Name")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Please enter the description")]
    [Display(Name = "Description")]
    public string Description { get; set; } = null!;

    [BindNever]
    public IdentityResult? Result { get; set; }
}
