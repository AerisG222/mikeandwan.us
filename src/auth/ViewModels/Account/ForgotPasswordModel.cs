﻿using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MawAuth.ViewModels.Account;

public class ForgotPasswordModel
{
    [Required(ErrorMessage = "Please enter your email address")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address")]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; } = null!;

    [BindNever]
    public bool WasEmailAttempted { get; set; }

    [BindNever]
    public bool WasSuccessful { get; set; }
}
