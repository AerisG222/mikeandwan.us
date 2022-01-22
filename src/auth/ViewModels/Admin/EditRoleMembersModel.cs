using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MawAuth.ViewModels.Admin;

public class EditRoleMembersModel
{
    [Required(ErrorMessage = "Please enter the role")]
    public string Role { get; set; } = null!;
    public IEnumerable<string> AllUsers { get; set; } = new List<string>();
    public IEnumerable<string> Members { get; set; } = new List<string>();
    public IEnumerable<string> NewMembers { get; set; } = new List<string>();

    [BindNever]
    public IdentityResult? Result { get; set; }
}
