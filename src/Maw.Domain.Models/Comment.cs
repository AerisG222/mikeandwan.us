using System;

namespace Maw.Domain.Models;

public class Comment
{
    public DateTime EntryDate { get; set; }
    public string CommentText { get; set; } = null!;
    public string Username { get; set; } = null!;
}
