using System;


namespace Maw.Domain.Photos
{
    public class Comment
    {
        public DateTime EntryDate { get; set; }
        public string CommentText { get; set; }
        public string Username { get; set; }
    }
}
