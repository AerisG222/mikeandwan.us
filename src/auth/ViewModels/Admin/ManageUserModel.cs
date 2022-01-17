using System;

namespace MawAuth.ViewModels.Admin;

public class ManageUserModel
{
    public string Username { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public DateTime? LastLoginDate { get; set; }
}
