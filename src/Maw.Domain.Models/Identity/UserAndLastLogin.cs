﻿namespace Maw.Domain.Models.Identity;

public class UserAndLastLogin
{
    public int? UserId { get; set; }
    public string? Username { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public DateTime? LastLoginDate { get; set; }
}
