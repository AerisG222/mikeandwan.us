using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MawAuth.ViewModels.Admin;

public class ManageRolesForUserModel
{
    readonly List<string> _allRoles = new();
    readonly List<string> _grantedRoles = new();

    [Required(ErrorMessage = "Please enter the username")]
    [Display(Name = "Username")]
    public string Username { get; set; } = null!;

    [BindNever]
    public IdentityResult? Result { get; set; }

    [BindNever]
    public List<string> AllRoles
    {
        get
        {
            return _allRoles;
        }
    }

    [BindNever]
    public List<string> GrantedRoles
    {
        get
        {
            return _grantedRoles;
        }
    }
}
