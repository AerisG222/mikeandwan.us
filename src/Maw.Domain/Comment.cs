using System;


namespace Maw.Domain
{
    public class Comment
    {
        public DateTime EntryDate { get; set; }
        public string CommentText { get; set; }
        public string Username { get; set; }
    }
}
