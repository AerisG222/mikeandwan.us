using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MawAuth.ViewModels.Admin;

public class EditRoleMembersModel
{
    [Required(ErrorMessage = "Please enter the role")]
    public string Role { get; set; }
    public IEnumerable<string> AllUsers { get; set; }
    public IEnumerable<string> Members { get; set; }
    public IEnumerable<string> NewMembers { get; set; }

    [BindNever]
    public IdentityResult Result { get; set; }

    public EditRoleMembersModel()
    {
        AllUsers = new List<string>();
        Members = new List<string>();
        NewMembers = new List<string>();
    }
}
