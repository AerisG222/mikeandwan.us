using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MawAuth.ViewModels.Account;

public class LoginModel
{
    [Required(ErrorMessage = "Please enter your username")]
    [Display(Name = "Username")]
    public string Username { get; set; } = null!;

    [Required(ErrorMessage = "Please enter your password")]
    [Display(Name = "Password")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;

    [BindNever]
    public IEnumerable<ExternalLoginScheme> ExternalSchemes { get; set; } = new List<ExternalLoginScheme>();

    [BindNever]
    public bool WasAttempted { get; set; }

    public string? ReturnUrl { get; set; }
}
