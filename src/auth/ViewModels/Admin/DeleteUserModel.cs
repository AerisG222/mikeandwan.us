using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MawAuth.ViewModels.Admin;

public class DeleteUserModel
{
    [Required]
    public string Username { get; set; } = null!;

    [BindNever]
    public IdentityResult? Result { get; set; }
}
