using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MawAuth.ViewModels.Account;

public class ChangePasswordModel
{
    [Required(ErrorMessage = "Please enter your current password")]
    [Display(Name = "Current Password")]
    [DataType(DataType.Password)]
    public string CurrentPassword { get; set; } = null!;

    [Required(ErrorMessage = "Please enter your new password")]
    [Display(Name = "New Password")]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; } = null!;

    [Required(ErrorMessage = "Please re-enter your new password")]
    [Display(Name = "Confirm New Password")]
    [DataType(DataType.Password)]
    [Compare(nameof(NewPassword), ErrorMessage = "Passwords did not match")]
    public string ConfirmPassword { get; set; } = null!;

    [BindNever]
    public bool ChangeAttempted { get; set; }

    [BindNever]
    public bool ChangeSucceeded { get; set; }
}
